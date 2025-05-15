# SauronVisionProtocol (SVP) - System Patterns

## System Architecture

The SauronVisionProtocol system follows a client-server architecture with the following high-level components:

```mermaid
graph TD
    subgraph "Client Applications"
        C[Console Client] --> P[Protocol Handler]
        M[macOS Client<br/>(planned)] --> P
        W[Windows Client<br/>(planned)] --> P
        P --> UI[User Interface]
    end
    
    subgraph "Azure Kubernetes Service (AKS)"
        subgraph "Server Pod"
            NL[.NET TCP Listener] --> CP[Command Processor]
            CP --> RG[Response Generator]
        end
    end
    
    P <--> NL
```

This architecture has been implemented and tested with the console client successfully connecting to the deployed server in Azure Kubernetes Service.

### Key Components

1. **Protocol Handler**: Implements the SVP protocol, handling the encoding/decoding of commands and responses between client UI and server.

2. **User Interface**: Platform-specific GUI implementations that provide consistent experience across supported platforms.

3. **.NET TCP Listener**: Component running on AKS that establishes and maintains TCP/IP socket connections using .NET 9's networking capabilities.

4. **Command Processor**: Interprets incoming commands according to the SVP specification.

5. **Response Generator**: Creates themed responses based on the processed commands.

## Design Patterns

1. **Command Pattern**: Used for encapsulating requests as objects, allowing for parameterization of clients with different requests and queue or log operations.

```
CommandInvoker (Client) -> Command Interface -> ConcreteCommand -> CommandReceiver (Server)
```

2. **Factory Pattern**: For creating platform-specific UI components while maintaining a consistent interface.

3. **Adapter Pattern**: To integrate with various Azure services and potentially different network libraries across platforms.

4. **Observer Pattern**: For UI updates based on connection status and command responses.

5. **Strategy Pattern**: For different command processing strategies on the server side.

## Communication Flow

1. Client establishes TCP/IP connection to server
2. Server acknowledges connection
3. Client sends command according to SVP format
4. Server processes command
5. Server generates response
6. Client receives and displays response
7. Connection remains open for additional commands (persistent connection model)

## Component Relationships

### Client Side

- **Platform Layer**: Contains platform-specific implementations
  - ✅ Implemented: Console-based client for cross-platform usage
  - Planned: Native UI rendering for macOS/Windows
  - Handles OS-specific networking considerations
  
- **Protocol Layer**: Platform-agnostic implementation of SVP
  - ✅ Implemented: TCP/IP client with event-based architecture
  - ✅ Implemented: Command formatting and transmission
  - ✅ Implemented: Response processing and parsing
  - ✅ Implemented: Connection state management
  
- **UI Layer**: Presents interface to users
  - ✅ Implemented: Console-based menu system
  - ✅ Implemented: Connection status display
  - ✅ Implemented: Command input mechanisms
  - ✅ Implemented: Response visualization
  - Planned: Graphical interface with three-panel layout

### Server Side

- **Listener Layer**: Accepts and manages TCP/IP connections
  - ✅ Implemented: Socket handling and connection management
  - ✅ Implemented: Connection lifecycle with proper cleanup
  - ✅ Implemented: Welcome message on connection establishment
  
- **Processing Layer**: Interprets and executes commands
  - ✅ Implemented: Command parsing and validation
  - ✅ Implemented: PALANTIR_GAZE command handling
  - ✅ Implemented: Error handling for invalid commands
  - Planned: Additional command implementations
  
- **Response Layer**: Generates themed responses
  - ✅ Implemented: Protocol-compliant response formatting
  - ✅ Implemented: Themed Lord of the Rings content generation
  - ✅ Implemented: Status codes and response types
  - Planned: Enhanced themed content and additional response types

## Technical Implementation Paths

### Azure Implementation Strategy

The project utilizes Azure Kubernetes Service (AKS) for the server-side implementation, with all infrastructure now successfully provisioned:

1. **Azure Kubernetes Service (AKS)** - ✅ Implemented:
   - Two-node cluster deployed with Standard_B2s VM size
   - Resource group: sauron-vision-protocol-rg
   - Cluster name: sauron-vision-protocol-aks
   - Leverages managed identity for secure access
   - Kubectl connected and verified with both nodes showing "Ready" status
   - Kubernetes version v1.31.7 deployed

2. **Azure Container Registry (ACR)** - ✅ Implemented:
   - Registry name: sauronvisionacr
   - Basic SKU for cost efficiency
   - Admin access enabled
   - Attached to AKS for pull access
   - Ready to store Docker container images for server components

3. **.NET 9 on Linux Containers**:
   - Server container implementation ready for deployment
   - Docker container configuration defined in Dockerfile
   - Kubernetes deployment manifests prepared
   - Implementation leverages .NET's TCP/IP socket capabilities

The implementation follows cloud-native best practices with infrastructure-as-code and prepared automated deployment processes. All Azure resources have been successfully provisioned and are ready for application deployment.

### Client Implementation Options

1. **Electron**:
   - Pros: Cross-platform, web technologies, rapid development
   - Cons: Resource usage, package size

2. **Flutter**:
   - Pros: Cross-platform, native performance, single codebase
   - Cons: Learning curve, relatively new for desktop

3. **Native Applications** (Swift for macOS, C#/WPF for Windows):
   - Pros: Best platform integration, performance
   - Cons: Separate codebases, higher maintenance

4. **React Native**:
   - Pros: JavaScript ecosystem, code sharing
   - Cons: Desktop support less mature than mobile

Initial client implementation approach to be determined based on team expertise and specific requirements.

## Protocol Design

SVP follows a text-based protocol format for simplicity of implementation and debugging, now implemented in the shared protocol models:

### Command Format - ✅ Implemented

Commands follow this format:
```
[COMMAND_NAME] [PARAM1] [PARAM2] ... [PARAMn]
```

Commands are Lord of the Rings/Sauron-themed. Currently implemented:

```
PALANTIR_GAZE [location]    # Directs the Eye of Sauron's gaze to a specific location
```

Planned future commands:
```
EYE_OF_SAURON [intensity] [duration]    # Controls the intensity of the gaze
RING_COMMAND [minion_type] [action]     # Commands minions to perform actions
```

### Response Format - ✅ Implemented

Responses follow this format:
```
[STATUS_CODE] [RESPONSE_TYPE] "[MESSAGE]"
```

Status codes:
- `200`: Success
- `500`: Error

Response types:
- `VISION_GRANTED`: The Eye of Sauron successfully directed its gaze
- `VISION_DENIED`: The Eye of Sauron could not or would not direct its gaze

Example:
```
200 VISION_GRANTED "The eye of Sauron turns to gondor. Armies of 5,000 orcs detected. The white city stands vulnerable."
```

A full protocol specification has been documented in `docs/protocol/specification.md` and implemented in the `shared/protocol/Models/` directory with Command and Response classes.

## Deployment Architecture

```mermaid
graph LR
    subgraph "Development"
        GR[GitHub Repository]
    end
    
    subgraph "CI/CD (GitHub Actions)"
        GR --> PathFilter[Path-based Filter]
        PathFilter -->|Server/Shared changes| Build[Build .NET Container]
        PathFilter -->|Other changes| Skip[Skip Build]
        Build --> Push[Push to ACR]
    end
    
    subgraph "Azure"
        Push --> ACR[Azure Container Registry]
        ACR --> Deploy[Deploy to AKS]
        Deploy --> AKS[Azure Kubernetes Service]
    end
    
    subgraph "Client Distribution"
        GR --> BuildClients[Build Client Apps]
        BuildClients --> CC[Console Client]
        BuildClients -.-> MC[macOS Client<br/>(planned)]
        BuildClients -.-> WC[Windows Client<br/>(planned)]
    end
```

This deployment architecture implements a fully automated CI/CD pipeline through GitHub Actions. The system now includes an optimized workflow with path-based filtering:

1. When changes are pushed to the repository, the workflow checks which files were modified
2. If server or shared protocol files are changed, the workflow triggers a build
3. GitHub Actions builds the Docker container for the server component
4. Pushes the container image to Azure Container Registry (ACR)
5. Updates the deployment on Azure Kubernetes Service (AKS)
6. If only client files are changed, the server build and deployment are skipped

This optimized approach provides:
- ✅ Efficient resource usage by avoiding unnecessary builds
- ✅ Faster feedback cycles for client-only changes
- ✅ Consistent, repeatable deployments for server components
- ✅ Version control for both code and container images
- ✅ Scalable infrastructure management through Kubernetes
- ✅ Clear separation between server-side and client-side deployment processes
- ✅ Manual trigger option for full deployments when needed

This pipeline has been successfully implemented and tested, with the server component deployed and verified in Azure Kubernetes Service.
