# SauronVisionProtocol (SVP) - Project Brief

## Project Overview

SauronVisionProtocol (SVP) is a proof-of-concept TCP/IP protocol designed with a Lord of the Rings / Sauron theme. The project includes a server component hosted on Azure and client applications that run on macOS and Windows.

## Core Requirements

1. **Protocol Design**:
   - Implement a custom TCP/IP protocol named "Sauron Vision Protocol"
   - Design a text-based protocol for readability and debugging
   - Create Sauron/LOTR-themed commands (PALANTIR_GAZE, EYE_OF_SAURON, etc.)
   - Support response codes, response types, and message content

2. **Server Component**:
   - Develop a server in .NET 9 running on Azure Kubernetes Service (AKS)
   - Implement socket handling for client connections
   - Process commands and return appropriate responses
   - Containerize with Docker for deployment to AKS

3. **Client Applications**:
   - Develop cross-platform client with Avalonia UI framework
   - Support macOS (with native Apple Silicon support) and Windows
   - Implement a three-panel GUI layout for protocol interaction
   - Use MVVM architecture with dependency injection

4. **Deployment & Infrastructure**:
   - Set up Azure resources (AKS, ACR, etc.)
   - Configure GitHub Actions for CI/CD pipeline
   - Create documentation for Azure setup and deployment
   - Support future extensibility

## Technical Constraints

1. **Platform Support**:
   - Server: Azure Kubernetes Service running Linux containers
   - Primary Client: macOS on Apple Silicon (M1/ARM64)
   - Secondary Client: Windows x64

2. **Technology Choices**:
   - Server: .NET 9, C#, containerized with Docker
   - Client: Avalonia UI (chosen for better Apple Silicon support over MAUI)
   - Protocol: Text-based TCP/IP custom protocol
   - Cloud: Azure Kubernetes Service (AKS)

3. **Compatibility Requirements**:
   - The protocol must be language-agnostic
   - Clients must run natively on both macOS and Windows 
   - Communication must be robust over unreliable networks

## Project Scope

### In Scope

1. **Protocol Development**:
   - Protocol specification document
   - Implementation of core commands (PALANTIR_GAZE, EYE_OF_SAURON, RING_COMMAND)
   - Error handling and response codes

2. **Server Implementation**:
   - Socket server implementation in .NET 9
   - Command processing logic
   - Docker containerization
   - Deployment to Azure Kubernetes Service

3. **Client Application**:
   - Avalonia UI cross-platform client
   - Connection management
   - Command submission interface
   - Response visualization

4. **Documentation**:
   - Protocol specification
   - Server deployment guide
   - Client installation instructions
   - Azure setup instructions

### Out of Scope

1. **Additional Features**:
   - User authentication (may be added in future)
   - End-to-end encryption (may be added in future)
   - Extended command set beyond initial proof-of-concept
   - Multiple simultaneous connections per client

2. **Alternative Clients**:
   - Mobile clients (iOS/Android)
   - Web interface
   - Command-line only client (though may be created for testing)

## Success Criteria

The project will be considered successful when:

1. The server successfully runs on Azure Kubernetes Service
2. The client applications run natively on both macOS (Apple Silicon) and Windows
3. The protocol allows for sending commands and receiving themed responses
4. The system demonstrates the three core commands working end-to-end
5. Documentation is complete for future extension and deployment

## Project Structure

```
SauronVisionProtocol/
├── client/
│   ├── avalonia/ (Cross-platform client with Avalonia UI)
│   ├── macos/ (Future macOS-specific extensions if needed)
│   └── windows/ (Future Windows-specific extensions if needed)
├── server/
│   ├── src/ (Server implementation)
│   ├── Dockerfile (Container definition)
│   └── kubernetes/ (Kubernetes deployment manifests)
├── shared/
│   └── protocol/ (Shared protocol definitions)
├── docs/
│   ├── protocol/ (Protocol documentation)
│   ├── azure-setup/ (Azure setup instructions) 
│   └── client-guides/ (Client installation guides)
└── .github/
    └── workflows/ (CI/CD pipeline definitions)
```

## Timeline

The project will be developed iteratively, with the following high-level milestones:

1. **Protocol Design & Planning**: Define protocol specification, architecture, and initial planning
2. **Initial Implementation**: Create basic client and server implementations with core functionality
3. **Azure Deployment**: Set up Azure infrastructure and deploy server component
4. **Testing & Refinement**: Test end-to-end functionality and refine implementation
5. **Documentation & Completion**: Finalize documentation and prepare for project handover/extension

## Future Considerations

While out of scope for the initial implementation, the following enhancements may be considered for future iterations:

1. Extended command set with more LOTR/Sauron-themed functionality
2. Enhanced visualization of Middle-earth locations and activities
3. Authentication and authorization layer
4. Protocol encryption for secure communication
5. Mobile client applications
6. Performance optimizations for handling larger numbers of connections
