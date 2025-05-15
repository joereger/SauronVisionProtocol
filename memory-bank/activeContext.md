# SauronVisionProtocol (SVP) - Active Context

## Current Work Focus

We are developing a proof-of-concept TCP/IP protocol called "Sauron Vision Protocol" with a client-server architecture. 

### May 15, 2025 - Initial Setup and Implementation

We have successfully:

1. Initialized the project structure with proper documentation using Memory Bank.
2. Selected our technology stack:
   - Server: .NET 9 on Azure Kubernetes Service
   - Client: Avalonia UI for cross-platform (macOS/Windows) compatibility
   - Protocol: Text-based TCP/IP custom protocol
3. Created initial client-side UI implementation:
   - Three-panel layout (Commands, Protocol Interaction, Connection Management)
   - MVVM architecture with proper dependency injection
   - Mock protocol client service for development
4. Defined the protocol structure:
   - Command format (command_name [parameters])
   - Response format (status_code response_type "message")
   - Three initial commands: PALANTIR_GAZE, EYE_OF_SAURON, RING_COMMAND

## Recent Changes

1. **Client Framework Decision**: We initially considered .NET MAUI but switched to Avalonia UI when we discovered MAUI has limitations on macOS (especially Apple Silicon).
   
2. **Protocol Design**: Implemented a text-based protocol with JSON-like structure for readability and debugging.
   
3. **UI Pattern**: Adopted a three-panel layout showing:
   - Available commands (left panel)
   - Command output and interaction (center panel)
   - Connection management (right panel)

4. **Protocol Implementation**: Created shared command and response models in the protocol library.

## Next Steps

### Immediate Tasks

1. **Implement real protocol client**:
   - Add real TCP/IP socket implementation
   - Connect to mock server for testing

2. **Server-side implementation**:
   - Set up basic server architecture in .NET 9
   - Create Docker container for deployment to AKS
   - Configure socket listening and command processing

3. **Protocol Refinement**:
   - Add validation and error handling
   - Implement proper command parsing
   - Add protocol versioning support

### Medium-Term Tasks

1. **Azure Setup**:
   - Deploy server to Azure Kubernetes Service
   - Set up monitoring and logging
   - Configure network security

2. **CI/CD Pipeline**:
   - Create GitHub Actions workflow
   - Automate deployment to Azure
   - Set up testing and validation

3. **Client Enhancements**:
   - Add theming with Lord of the Rings / Sauron aesthetic
   - Improve visualization of command responses
   - Support for command history and favorites

## Active Decisions and Considerations

1. **Protocol Format Decision**: 
   - Using a text-based format for better debugging and readability
   - Format: `COMMAND_NAME parameter1 parameter2` for requests
   - Format: `status_code RESPONSE_TYPE "message"` for responses
   - Example: `200 VISION_GRANTED "The eye of Sauron turns to gondor. Armies detected."`

2. **Cross-Platform Strategy**:
   - Avalonia UI selected for better native support on macOS Apple Silicon
   - Shared protocol library (.NET Standard) for client/server code reuse
   - Containerized server deployment for platform independence

3. **MVVM Implementation**:
   - Using proper dependency injection for loosely coupled components
   - Mock service implementation for client development without server
   - Command bindings for UI interactions
   - Observable collections and properties for reactive UI

4. **Lord of the Rings Theming**:
   - Command naming convention based on Sauron's abilities
   - Response messages themed around Middle-earth locations and characters
   - Will eventually add visual theming to match the aesthetic

## Important Patterns and Preferences

1. **Coding Patterns**:
   - Use dependency injection for all services
   - Leverage MVVM pattern for UI separation of concerns
   - Implement both mock and real services with shared interfaces
   - Use strongly-typed models for protocol messages

2. **Architecture Preferences**:
   - Separation between protocol, client, and server components
   - Shared code in protocol library
   - Client-side reactive UI patterns
   - Server-side command handling with proper error management

3. **Project Structure**:
   - `/client/avalonia`: Avalonia UI client for macOS and Windows
   - `/shared/protocol`: Shared protocol models and utilities
   - `/server`: Server implementation (to be created)

## Learnings and Project Insights

1. **.NET MAUI vs Avalonia**: We learned that Avalonia offers better support for macOS Apple Silicon than .NET MAUI, which has limitations and requires special workloads.

2. **TCP/IP in Azure**: Discovered that Azure Kubernetes Service is better suited for our custom TCP/IP protocol than serverless options like Azure Functions.

3. **Protocol Design**: Text-based protocols are easier to debug and extend compared to binary protocols, which is valuable for a proof-of-concept.

4. **UI Architecture**: Using MVVM with dependency injection allows for easier testing and separation between UI and business logic.
