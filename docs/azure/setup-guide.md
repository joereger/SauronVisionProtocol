# Azure Setup Guide for SauronVisionProtocol

This guide walks through setting up the necessary Azure resources for deploying the SauronVisionProtocol server.

## Prerequisites

- Azure account with an active subscription
- Azure CLI installed on your development machine (see below for installation)
- `kubectl` command-line tool (see below for installation)

### Installing Azure CLI on macOS

```bash
# Using Homebrew (recommended)
brew update
brew install azure-cli

# Verify installation
az --version
```

For other installation methods, see the [official Azure CLI documentation](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli).

### Installing kubectl on macOS

```bash
# Using Homebrew (recommended)
brew install kubectl

# Verify installation
kubectl version --client
```

Alternatively, you can install kubectl directly:

```bash
# Download the latest release
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/darwin/amd64/kubectl"

# Make the kubectl binary executable
chmod +x ./kubectl

# Move kubectl to a directory in your PATH
sudo mv ./kubectl /usr/local/bin/kubectl

# Verify the installation
kubectl version --client
```

For Windows or Linux installation instructions, see the [official Kubernetes documentation](https://kubernetes.io/docs/tasks/tools/).

## 1. Resource Provider Registration

Before creating Azure resources, you need to register the required resource providers for your subscription. This is a one-time process for each subscription:

```bash
# Check registration status of required providers
az provider show -n Microsoft.ContainerRegistry --query "registrationState" -o tsv
az provider show -n Microsoft.ContainerService --query "registrationState" -o tsv
az provider show -n Microsoft.Network --query "registrationState" -o tsv
az provider show -n Microsoft.Compute --query "registrationState" -o tsv
az provider show -n Microsoft.Storage --query "registrationState" -o tsv
```

For any provider showing "NotRegistered", register them using:

```bash
# Register Container Registry provider (for ACR)
az provider register --namespace Microsoft.ContainerRegistry

# Register Container Service provider (for AKS)
az provider register --namespace Microsoft.ContainerService

# Register additional providers required by AKS
az provider register --namespace Microsoft.Network
az provider register --namespace Microsoft.Compute
az provider register --namespace Microsoft.Storage
```

Registration takes 2-5 minutes for each provider. Check status with:

```bash
az provider show -n Microsoft.ContainerRegistry --query "registrationState" -o tsv
# Repeat for other providers
```

Wait for all providers to show "Registered" before proceeding.

## 2. Create Resource Group

First, create a resource group to contain all related resources:

```bash
# Login to Azure
az login

# Create resource group
az group create --name sauron-vision-protocol-rg --location eastus
```

## 3. Create Azure Container Registry (ACR)

Create an ACR to store your Docker container images:

```bash
# Create ACR (name must be globally unique)
az acr create \
  --resource-group sauron-vision-protocol-rg \
  --name sauronvisionacr \
  --sku Basic \
  --admin-enabled true
```

Get the ACR credentials for GitHub Actions:

```bash
# Get ACR login server
ACR_SERVER=$(az acr show --name sauronvisionacr --resource-group sauron-vision-protocol-rg --query loginServer --output tsv)
echo "ACR Server: $ACR_SERVER"

# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name sauronvisionacr --resource-group sauron-vision-protocol-rg --query username --output tsv)
ACR_PASSWORD=$(az acr credential show --name sauronvisionacr --resource-group sauron-vision-protocol-rg --query "passwords[0].value" --output tsv)

echo "ACR Username: $ACR_USERNAME"
echo "ACR Password: $ACR_PASSWORD"
```

## 4. Create Azure Kubernetes Service (AKS) Cluster

Create an AKS cluster for running the service:

```bash
# Create AKS cluster
az aks create \
  --resource-group sauron-vision-protocol-rg \
  --name sauron-vision-protocol-aks \
  --node-count 2 \
  --node-vm-size Standard_B2s \
  --enable-managed-identity \
  --attach-acr sauronvisionacr \
  --generate-ssh-keys
```

Configure `kubectl` to connect to your cluster:

```bash
# Get AKS credentials
az aks get-credentials \
  --resource-group sauron-vision-protocol-rg \
  --name sauron-vision-protocol-aks
```

Verify connectivity:

```bash
kubectl get nodes
```

## 5. Create Service Principal for GitHub Actions

To allow GitHub Actions to deploy to AKS, create a service principal:

```bash
# Create service principal with Contributor role on resource group
az ad sp create-for-rbac \
  --name "sauron-vision-github-actions" \
  --role Contributor \
  --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/sauron-vision-protocol-rg \
  --sdk-auth
```

The output JSON contains the credentials needed for GitHub Actions.

## 6. Set Up GitHub Secrets

You need to store the credentials from the service principal and ACR as GitHub secrets for your CI/CD pipeline:

### Accessing GitHub Secrets

1. Go to your GitHub repository
2. Navigate to "Settings" > "Secrets and variables" > "Actions"
3. Click "New repository secret" button

### Required Secrets

Add the following **repository secrets** (not environment secrets):

- `AZURE_CREDENTIALS`: The entire JSON output from the service principal creation (copy and paste the complete JSON including curly braces)
- `AZURE_SUBSCRIPTION`: Your subscription ID (from the output of `az account show --query id -o tsv`)
- `AZURE_RESOURCE_GROUP`: The resource group name (`sauron-vision-protocol-rg`)
- `AKS_CLUSTER_NAME`: The AKS cluster name (`sauron-vision-protocol-aks`)

### ACR Credentials

Run these commands to get your ACR credentials:

```bash
# Get ACR login server
ACR_SERVER=$(az acr show --name sauronvisionacr --resource-group sauron-vision-protocol-rg --query loginServer --output tsv)
echo "ACR Server: $ACR_SERVER"

# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name sauronvisionacr --resource-group sauron-vision-protocol-rg --query username --output tsv)
ACR_PASSWORD=$(az acr credential show --name sauronvisionacr --resource-group sauron-vision-protocol-rg --query "passwords[0].value" --output tsv)

echo "ACR Username: $ACR_USERNAME"
echo "ACR Password: $ACR_PASSWORD"
```

Then add these as GitHub secrets as well:
- `ACR_SERVER`: The ACR login server URL (typically `sauronvisionacr.azurecr.io`)
- `ACR_USERNAME`: The ACR username (typically the ACR name)
- `ACR_PASSWORD`: The ACR password

### Security Considerations

- These secrets are encrypted in GitHub and are only exposed during workflow runs
- Treat the service principal credentials as sensitive information
- Avoid displaying these credentials in logs or sharing them in public repositories

## 7. Configure Network Settings

Our TCP/IP server needs to be accessible from external clients. Kubernetes provides several ways to expose services, with LoadBalancer being the recommended approach for Azure:

### Creating a LoadBalancer Service

1. Create or review the Kubernetes service configuration in `server/kubernetes/service.yaml`:
   ```yaml
   apiVersion: v1
   kind: Service
   metadata:
     name: sauronvisionprotocol
     labels:
       app: sauronvisionprotocol
   spec:
     type: LoadBalancer
     ports:
     - port: 9000
       targetPort: 9000
       protocol: TCP
       name: tcp-socket
     selector:
       app: sauronvisionprotocol
   ```

2. Apply the service configuration:
   ```bash
   kubectl apply -f server/kubernetes/service.yaml
   ```

3. Get the external IP address (this may take a few minutes to provision):
   ```bash
   kubectl get services sauronvisionprotocol
   ```
   Look for the "EXTERNAL-IP" column. Initially, it might show `<pending>` while Azure provisions the load balancer.

4. Once provisioned, you can connect to the TCP/IP server at this external IP on port 9000, and test connectivity with:
   ```bash
   nc -vz [EXTERNAL-IP] 9000
   ```

### Network Security Considerations

By default, the LoadBalancer exposes your service to the public internet. For production environments, consider:

- Adding network policies to restrict traffic
- Using Azure Private Link for private connectivity
- Implementing TLS for encrypted communications
- Setting up Azure Firewall to control inbound/outbound traffic

### Troubleshooting Network Connectivity

If you have issues connecting to the service:

1. Check service status:
   ```bash
   kubectl describe service sauronvisionprotocol
   ```

2. Ensure the pod is running:
   ```bash
   kubectl get pods --selector=app=sauronvisionprotocol
   ```

3. Check pod logs:
   ```bash
   kubectl logs [pod-name]
   ```

4. Verify that your AKS Network Security Group (NSG) allows traffic on port 9000

## 8. Monitoring Setup

Monitoring is essential for production workloads to track performance, detect issues, and gather insights about your application's operations.

### Enable Azure Monitor for Containers

Enable monitoring for your AKS cluster:

```bash
# Enable monitoring
az aks enable-addons \
  --resource-group sauron-vision-protocol-rg \
  --name sauron-vision-protocol-aks \
  --addons monitoring
```

This integrates Azure Monitor and logs analytics with your AKS cluster, collecting:
- Container metrics (CPU, memory, etc.)
- Container logs
- Node performance metrics
- Control plane logs

### Viewing Monitoring Data

1. Access the monitoring dashboard in the Azure Portal:
   - Go to your Azure Portal
   - Navigate to your AKS cluster resource
   - Select "Insights" or "Monitoring" from the left menu

2. Key monitoring views:
   - **Cluster metrics**: Overall CPU/memory usage across all nodes
   - **Node performance**: Metrics for individual nodes
   - **Container logs**: Application-specific logs
   - **Kubernetes events**: System-level events for troubleshooting

### Setting Up Alerts

You can configure alerts based on metrics thresholds:

```bash
# Example: Create an alert for high CPU usage
az monitor metrics alert create \
  --name "aks-high-cpu-alert" \
  --resource-group sauron-vision-protocol-rg \
  --scopes $(az aks show -g sauron-vision-protocol-rg -n sauron-vision-protocol-aks --query id -o tsv) \
  --condition "avg percentage CPU > 80" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --description "Alert when CPU exceeds 80% for 5 minutes"
```

### Logging for SauronVisionProtocol

Our server application is configured to use structured logging (Serilog) that integrates well with Azure Monitor. Key logging areas:

- Connection events (client connect/disconnect)
- Command processing
- Error conditions
- Performance metrics

## 9. Cleanup (When No Longer Needed)

To clean up all resources when they're no longer needed:

```bash
# Delete resource group and all contained resources
az group delete --name sauron-vision-protocol-rg --yes --no-wait
```

## 10. Troubleshooting

### Common Issues and Solutions

#### Resource Provider Registration Errors

If you see an error like:
```
(MissingSubscriptionRegistration) The subscription is not registered to use namespace 'Microsoft.X'
```

This means the required resource provider isn't registered. Follow the registration steps in section 1. Each registration can take several minutes to complete.

#### AKS Creation Takes a Long Time

Creating an AKS cluster typically takes 5-15 minutes. During this time, Azure is provisioning VMs, networking resources, and configuring Kubernetes components.

#### Command Line Continuation with Backslashes

When using multi-line commands with backslashes (`\`), ensure there are no spaces after the backslash, as this can cause command syntax errors in some shells.

#### ResourceNotFound Errors

If a "ResourceNotFound" error occurs when trying to access a resource you just created, it might still be provisioning. Wait a few minutes and try again.

#### Permissions Issues

If you encounter permissions errors, ensure your Azure account has sufficient permissions (Contributor or Owner role) on the subscription.

## 11. Cost Management

The resources deployed in this guide include:

- Azure Container Registry (Basic tier): ~$5/month
- AKS Cluster with 2 Standard_B2s nodes: ~$70-80/month
- Associated storage and networking: ~$10-20/month

Total estimated cost: ~$85-105/month

For development and testing purposes, you can:
- Use a single-node AKS cluster
- Stop the cluster when not in use
- Use a free tier ACR if you don't need high availability

## 12. Conclusion and Next Steps

Congratulations! You've now set up the complete Azure infrastructure for the SauronVisionProtocol project. Here's a summary of what you've accomplished:

- Registered required Azure resource providers
- Created a resource group for project resources
- Provisioned an Azure Container Registry for Docker images
- Deployed an Azure Kubernetes Service cluster
- Created a service principal for GitHub Actions
- Configured GitHub secrets for CI/CD pipeline
- Set up network configuration for TCP/IP communication
- Enabled monitoring for the AKS cluster

### Next Steps

1. **Deploy the Server Component**:
   ```bash
   # Navigate to the server directory
   cd server
   
   # Apply the Kubernetes deployment and service
   kubectl apply -f kubernetes/deployment.yaml
   kubectl apply -f kubernetes/service.yaml
   
   # Check deployment status
   kubectl get deployments
   kubectl get services
   ```

2. **Set Up CI/CD Pipeline**:
   - Ensure your GitHub Actions workflow (`.github/workflows/build-deploy.yml`) is configured correctly
   - Push code changes to trigger the CI/CD pipeline
   - Monitor the GitHub Actions tab for build and deployment status

3. **Test the Deployed Server**:
   - Get the external IP of your service:
     ```bash
     kubectl get services sauronvisionprotocol
     ```
   - Test connectivity with the server using the test script:
     ```bash
     ./server/test-client.sh [EXTERNAL-IP] 9000
     ```

4. **Develop the Client Application**:
   - Configure client to connect to the server's external IP
   - Test protocol communication end-to-end

5. **Refine and Extend**:
   - Add more commands to the protocol
   - Enhance monitoring and alerting
   - Implement security improvements
