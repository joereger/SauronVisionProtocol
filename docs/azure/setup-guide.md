# Azure Setup Guide for SauronVisionProtocol

This guide walks through setting up the necessary Azure resources for deploying the SauronVisionProtocol server.

## Prerequisites

- Azure account with an active subscription
- Azure CLI installed on your development machine
- `kubectl` command-line tool

## 1. Create Resource Group

First, create a resource group to contain all related resources:

```bash
# Login to Azure
az login

# Create resource group
az group create --name sauron-vision-protocol-rg --location eastus
```

## 2. Create Azure Container Registry (ACR)

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

## 3. Create Azure Kubernetes Service (AKS) Cluster

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

## 4. Create Service Principal for GitHub Actions

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

## 5. Set Up GitHub Secrets

In your GitHub repository, add the following secrets:

- `AZURE_CREDENTIALS`: The entire JSON output from the service principal creation
- `ACR_SERVER`: The ACR login server URL
- `ACR_USERNAME`: The ACR username
- `ACR_PASSWORD`: The ACR password
- `AZURE_RESOURCE_GROUP`: The resource group name (`sauron-vision-protocol-rg`)
- `AKS_CLUSTER_NAME`: The AKS cluster name (`sauron-vision-protocol-aks`)

## 6. Configure Network Settings

By default, our Kubernetes service is configured to create a LoadBalancer service, which will automatically provision an external IP address for accessing the TCP/IP server.

## 7. Monitoring Setup

Enable monitoring for your AKS cluster:

```bash
# Enable monitoring
az aks enable-addons \
  --resource-group sauron-vision-protocol-rg \
  --name sauron-vision-protocol-aks \
  --addons monitoring
```

## 8. Cleanup (When No Longer Needed)

To clean up all resources when they're no longer needed:

```bash
# Delete resource group and all contained resources
az group delete --name sauron-vision-protocol-rg --yes --no-wait
```

## 9. Cost Management

The resources deployed in this guide include:

- Azure Container Registry (Basic tier): ~$5/month
- AKS Cluster with 2 Standard_B2s nodes: ~$70-80/month
- Associated storage and networking: ~$10-20/month

Total estimated cost: ~$85-105/month

For development and testing purposes, you can:
- Use a single-node AKS cluster
- Stop the cluster when not in use
- Use a free tier ACR if you don't need high availability
