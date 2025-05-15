# SauronVisionProtocol (SVP) - Active Context

## Current Work Focus

The SauronVisionProtocol project is in its initial planning and setup phase. The current focus is on:

1. **Project Structure Setup**: Establishing the foundational directory structure and documentation.

2. **GitHub Actions CI/CD Pipeline**: Setting up the automated build and deployment workflow to AKS, which is critical for development without all local runtimes.

3. **.NET 9 Server Implementation**: Designing a simple containerized .NET 9 TCP/IP server application to run on AKS, starting with a single command.

4. **Client Application Framework**: Setting up the .NET MAUI client application with the three-panel layout.

5. **Protocol Definition**: Implementing a basic version of the protocol with a single command for initial testing.

## Recent Changes

1. **Project Initialization**: Created the project repository and established the Memory Bank documentation system.

2. **Documentation Creation**: Developed initial versions of project documentation including requirements, architecture, and technical context.

3. **Technology Selection**: 
   - Selected Azure Kubernetes Service (AKS) with .NET 9 on Linux containers for the server implementation
   - Chosen .NET MAUI for cross-platform client application development
   - Defined a three-panel UI layout for protocol visualization

4. **Deployment Strategy**: Defined a GitHub Actions based CI/CD pipeline for automated deployment to AKS.

5. **GitHub Repository Setup**: Initialized local Git repository and published to GitHub.

## Next Steps

Immediate next steps include:

1. **GitHub Actions CI/CD Pipeline Setup**: 
   - Create GitHub Actions workflow files
   - Set up Docker container build process
   - Configure Azure connectivity and authentication
   - Implement deployment to AKS

2. **Azure Infrastructure Setup**:
   - Create Azure Container Registry (ACR)
   - Set up Azure Kubernetes Service (AKS) cluster
   - Configure network settings for TCP/IP socket exposure

3. **Project Structure Implementation**:
   - Create the directory structure outlined in README.md
   - Set up the server project with Docker support
   - Set up the client .NET MAUI project
   - Create shared protocol definition libraries

4. **Minimal Viable Implementation**:
   - Implement a simple TCP/IP server with a single command handler
   - Create a basic .NET MAUI client with the three-panel layout
   - Implement protocol visualization for the initial command
   - Test end-to-end communication

5. **Documentation Development**:
   - Create setup guides for Azure resources
   - Document the deployment process
   - Create protocol specification for the initial command

## Active Decisions and Considerations

Current decisions being evaluated:

1. **Azure Infrastructure Configuration**:
   - **Decision Needed**: Optimal configuration for AKS cluster to support TCP/IP socket server
   - **Considerations**: Node size, scaling policies, network configuration, load balancing
   - **Current Leaning**: Start with minimal configuration focused on stability and cost-efficiency, then scale as needed

2. **Client UI Implementation**:
   - **Decision Made**: .NET MAUI with three-panel layout
   - **Considerations**: Modern, clean UI with subtle themed elements, focus on protocol visualization
   - **Implementation Approach**: Start with basic layout and functionality, refine visual design iteratively

3. **CI/CD Pipeline Structure**:
   - **Decision Needed**: How to structure the GitHub Actions workflow for optimal efficiency
   - **Considerations**: Build speed, test coverage, environment separation, security practices
   - **Current Leaning**: Implement CI/CD pipeline early with emphasis on automation and repeatability

4. **Protocol Format**:
   - **Decision Made**: Text-based protocol format
   - **Initial Implementation**: Single command (PALANTIR_GAZE) for simplicity
   - **Visualization Approach**: Raw byte display with decoded command/response format

## Important Patterns and Preferences

1. **Code Organization**: Clear separation between client and server code, with shared protocol definitions in a common module.

2. **Documentation First**: Defining specifications and interfaces before implementation to ensure clarity and alignment.

3. **Consistent Theming**: All components should maintain the Lord of the Rings/Sauron theme in naming, messaging, and visual elements.

4. **Modular Architecture**: Designing components with clear interfaces to allow for future extensions or replacements.

5. **Test-Driven Development**: Creating tests alongside feature implementation to ensure reliability and facilitate refactoring.

## Learnings and Project Insights

As the project is in its early stages, key learnings will be documented here as they emerge. Initial observations include:

1. **Development Environment Challenges**: Development on macOS without all runtimes locally installed reinforces the importance of a robust CI/CD pipeline for testing and deployment.

2. **Kubernetes for TCP/IP Services**: Azure Kubernetes Service provides a more suitable platform for TCP/IP socket servers compared to serverless options, due to persistent connection requirements.

3. **.NET Ecosystem Integration**: Leveraging .NET 9 for server and .NET MAUI for client allows for efficient code sharing and consistent development practices.

4. **GitHub Actions Automation**: Early investment in CI/CD automation will pay dividends throughout the project lifecycle, especially given the development environment constraints.

5. **Incremental Implementation Strategy**: Starting with minimal functionality and focusing on deployment infrastructure reduces debugging complexity in early stages.

6. **Technical Visualization Needs**: For protocol implementations, technical users need visibility into the raw data alongside the higher-level abstracted interfaces.

7. **Three-Panel Layout Approach**: Organizing the UI into client, protocol, and server panels provides intuitive visualization of the entire system and data flow.
