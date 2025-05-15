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
3. Created functional client-side UI implementation:
   - Three-panel layout (Commands, Protocol Interaction, Connection Management)
   - MVVM architecture with proper dependency injection
   - Mock protocol client service for development
4. Defined and implemented the protocol structure:
   - Command format (command_name [parameters])
   - Response format (status_code response_type "message")
   - Three initial commands: PALANTIR_GAZE, EYE_OF_SAURON, RING_COMMAND
5. Successfully established communication between client and Azure server.
6. Fixed server-side implementation to use the updated protocol model.
7. **Completed UI Enhancements**:
   - Restructured to a three-panel layout with equal column widths.
   - Moved command input to the left panel.
   - Updated column headers ("Local Avalonia App", "SVP Communications", "Server Azure App").
   - Successfully implemented animated GIF images (Eye of Sauron) for connection status, using correct Avalonia resource paths (`avares://`).

## Recent Changes

1. **Client Framework Decision**: We initially considered .NET MAUI but switched to Avalonia UI when we discovered MAUI has limitations on macOS (especially Apple Silicon).
   
2. **Protocol Design**: Implemented a text-based protocol with structured format for readability and debugging.
   
3. **UI Pattern**: Adopted a three-panel layout showing:
   - Available commands and command input (left panel)
   - Protocol communication log (center panel)
   - Connection management and server status (right panel)

4. **Protocol Implementation**: Created shared command and response models in the protocol library.

5. **Server Connection**: Updated client defaults to connect to the Azure-hosted server (sauronvisionprotocol.eastus.cloudapp.azure.com:9000).

6. **Protocol Enhancement**: Extended the protocol with additional themed responses based on command types and parameters.

7. **Image Display Fix**: Resolved issues with displaying images in Avalonia by:
    - Switching from WebP to GIF format.
    - Using the `avares://<AssemblyName>/<PathToResource>` URI scheme for image sources.
    - Ensuring images are included as `AvaloniaResource` in the `.csproj` file.
    - Simplifying XAML by removing problematic converters and using direct `Image` controls with `IsVisible` binding.

## Next Steps

### Immediate Tasks

2. **Protocol Implementation Completion**:
   - Implement real TCP/IP socket client (replacing mock implementation)
   - Add comprehensive error handling
   - Add support for all command types

3. **Server Enhancement**:
   - Improve logging and telemetry
   - Implement more sophisticated command handling
   - Add support for concurrent client connections

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
