# SauronVisionProtocol (SVP) - System Patterns

## System Architecture

The SauronVisionProtocol system follows a client-server architecture with the following high-level components:

```mermaid
graph TD
    subgraph "Client Applications"
        M[macOS Client] --> P[Protocol Handler]
        W[Windows Client] --> P
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

- **Platform Layer**: Contains platform-specific implementations (macOS/Windows)
  - Responsible for native UI rendering
  - Handles OS-specific networking considerations
  
- **Protocol Layer**: Platform-agnostic implementation of SVP
  - Handles command formatting and transmission
  - Processes received responses
  - Manages connection state

- **UI Layer**: Presents themed interface to users
  - Displays connection status
  - Provides command input mechanisms
  - Renders server responses

### Server Side

- **Listener Layer**: Accepts and manages TCP/IP connections
  - Implements socket handling
  - Manages connection lifecycle
  
- **Processing Layer**: Interprets and executes commands
  - Validates command format
  - Executes business logic based on command type
  
- **Response Layer**: Generates themed responses
  - Formats data according to protocol
  - Adds thematic elements to responses

## Technical Implementation Paths

### Azure Implementation Strategy

The project will utilize Azure Kubernetes Service (AKS) for the server-side implementation:

1. **Azure Kubernetes Service (AKS)**:
   - Pros: 
     - Excellent support for containerized applications
     - Robust orchestration capabilities
     - Scalable architecture
     - Support for persistent TCP/IP connections
     - Mature monitoring and management tools
   - Cons:
     - More complex initial setup than serverless options
     - Requires Kubernetes knowledge
     - Potentially higher base cost than serverless for low-traffic scenarios

2. **Azure Container Registry (ACR)**:
   - Used for storing and managing Docker container images
   - Integrated with the CI/CD pipeline via GitHub Actions
   - Provides version control for container images

3. **.NET 9 on Linux Containers**:
   - Leverages .NET's cross-platform capabilities
   - Utilizes native .NET TCP/IP socket implementations
   - Runs efficiently in containerized environments

The implementation will follow cloud-native best practices with infrastructure-as-code and automated deployment processes.

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

SVP follows a text-based protocol format for simplicity of implementation and debugging:

```
[COMMAND_NAME] [PARAM1] [PARAM2] ... [PARAMn]
```

Commands will be Lord of the Rings/Sauron-themed, such as:

```
PALANTIR_GAZE target_location
EYE_OF_SAURON scan_intensity scan_duration
RING_COMMAND minion_type action_type
```

Responses follow a similar format:

```
[STATUS_CODE] [RESPONSE_TYPE] [MESSAGE]
```

Example:
```
200 VISION_GRANTED "The eye of Sauron turns to Gondor. Armies of 5000 orcs detected."
```

A full protocol specification will be developed as the project progresses.

## Deployment Architecture

```mermaid
graph LR
    subgraph "Development"
        GR[GitHub Repository]
    end
    
    subgraph "CI/CD (GitHub Actions)"
        GR --> Build[Build .NET Container]
        Build --> Test[Run Tests]
        Test --> Push[Push to ACR]
    end
    
    subgraph "Azure"
        Push --> ACR[Azure Container Registry]
        ACR --> Deploy[Deploy to AKS]
        Deploy --> AKS[Azure Kubernetes Service]
    end
    
    subgraph "Client Distribution"
        GR --> BuildClients[Build Client Apps]
        BuildClients --> MC[macOS Client]
        BuildClients --> WC[Windows Client]
    end
```

This deployment architecture implements a fully automated CI/CD pipeline through GitHub Actions. When changes are pushed to the repository:

1. GitHub Actions automatically builds the Docker container for the server component
2. Runs automated tests to verify functionality
3. Pushes the container image to Azure Container Registry (ACR)
4. Updates the deployment on Azure Kubernetes Service (AKS)

This approach provides:
- Consistent, repeatable deployments
- Version control for both code and container images
- Automated testing as part of the deployment pipeline
- Scalable infrastructure management through Kubernetes
- Separation between server-side and client-side deployment processes

Since development will be primarily on macOS without all runtimes installed locally, this pipeline is essential for testing and deployment.
