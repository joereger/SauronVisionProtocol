# Developer Guide for SauronVisionProtocol

This guide helps developers set up their environment and start working with the SauronVisionProtocol project.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop/) (optional, for local container testing)
- [Visual Studio Code](https://code.visualstudio.com/) or another IDE
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)
- Git

## Project Structure

The SauronVisionProtocol is organized into the following directories:

- `.github/workflows/`: Contains GitHub Actions workflow definitions
- `client/`: .NET MAUI client application
- `docs/`: Documentation
- `memory-bank/`: Project memory documentation
- `server/`: .NET 9 TCP/IP server implementation
- `shared/`: Shared code between client and server

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/SauronVisionProtocol.git
cd SauronVisionProtocol
```

### 2. Build the Server

```bash
cd server/src
dotnet restore
dotnet build
```

### 3. Run the Server Locally

```bash
cd server/src
dotnet run
```

The server will start listening on port 9000 by default. You can change this by setting the `Port` environment variable or updating `appsettings.json`.

### 4. Test the Server

While the server is running, open another terminal and run the test client script:

```bash
cd server
chmod +x test-client.sh
./test-client.sh localhost 9000
```

You should see commands being sent and responses received.

### 5. Build and Run in Docker (Optional)

From the project root:

```bash
cd server
docker build -t sauronvisionprotocol:dev .
docker run -p 9000:9000 sauronvisionprotocol:dev
```

## Development Workflow

1. **Make Changes**: Modify code in your local environment
2. **Test Locally**: Run the server and test with the test client
3. **Commit and Push**: Push changes to GitHub
4. **CI/CD**: GitHub Actions will automatically build and deploy the changes

## Server Development

The server component uses the following key files:

- `Program.cs`: Application entry point and dependency injection setup
- `Services/ITcpServer.cs`: Interface for the TCP server
- `Services/TcpServer.cs`: Implementation of the TCP server
- `Services/ICommandProcessor.cs`: Interface for command processing
- `Services/CommandProcessor.cs`: Implementation of command processing
- `Services/TcpServerHostedService.cs`: Hosted service to manage the TCP server lifecycle

To add new commands:

1. Add the command class in `shared/protocol/Models/Command.cs`
2. Update the `FromProtocolString` method to parse the new command
3. Add a new command handler method in `CommandProcessor.cs`
4. Update the switch statement in `ProcessCommandAsync` to call your new handler

## Client Development

The .NET MAUI client application will be implemented in the `client/` directory. The client features a three-panel layout:

1. Left Panel: Client interface for command input
2. Middle Panel: Protocol visualization showing raw data exchange
3. Right Panel: Server response visualization with Sauron theming

Follow the standard .NET MAUI development practices for implementing the client.

## Deployment

Deployment is handled through GitHub Actions and Azure Kubernetes Service. See the Azure setup guide in `docs/azure/setup-guide.md` for details on setting up the required Azure resources.

The key files for deployment are:

- `.github/workflows/build-deploy.yml`: CI/CD workflow definition
- `server/kubernetes/deployment.yaml`: Kubernetes deployment manifest
- `server/kubernetes/service.yaml`: Kubernetes service manifest
- `server/Dockerfile`: Docker build definition

## Documentation

Please keep the documentation up to date when making changes:

- Update `docs/protocol/specification.md` when changing the protocol
- Add/update setup instructions in `docs/setup/` when changing the setup process
- Update Azure-related documentation in `docs/azure/` for cloud changes

## Memory Bank

This project uses a Memory Bank system to maintain context across development sessions. See the files in the `memory-bank/` directory to understand the project's history, architecture, and technical approach.
