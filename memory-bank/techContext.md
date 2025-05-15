# SauronVisionProtocol (SVP) - Technical Context

## Technologies Used

### Server-Side Technologies

1. **Azure Platform**:
   - Containerized architecture using Azure Kubernetes Service (AKS)
   - Azure Container Registry (ACR) for Docker image storage
   - Kubernetes for container orchestration

2. **Backend Development**:
   - .NET 9 for server-side implementation
   - C# for programming language
   - Native TCP/IP socket handling capabilities

3. **Cloud Infrastructure**:
   - Containerization with Docker (Linux-based containers)
   - Kubernetes manifests for deployment configuration
   - GitHub Actions for CI/CD pipeline integration
   - Azure Monitor for logging and performance tracking

### Client-Side Technologies

1. **Cross-Platform Framework**:
   - To be determined based on compatibility with .NET 9 backend
   - Focus on macOS development initially, with Windows support planned

2. **UI/UX**:
   - Modern UI framework (React if using Electron, or platform-appropriate UI toolkit)
   - Responsive design to accommodate different window sizes
   - Themed visual components aligned with Lord of the Rings/Sauron aesthetic

3. **Networking**:
   - Platform-appropriate socket libraries
   - Connection state management
   - Error handling and retry logic

## Development Setup

### Environment Requirements

1. **Development Machine**:
   - macOS for primary development and testing
   - Windows for cross-platform testing
   - Git for version control
   - Visual Studio Code as recommended IDE with appropriate extensions

2. **Required Software**:
   - Node.js (latest LTS version)
   - npm or Yarn package manager
   - Azure CLI for cloud resource management
   - Platform SDKs as needed (Xcode for macOS, Visual Studio for Windows)

3. **Local Testing Tools**:
   - Local TCP/IP socket testing utilities
   - Network traffic analyzers (e.g., Wireshark) for protocol debugging
   - Mock server for offline client development

### Development Workflow

1. **Setup Process**:
   ```bash
   # Clone repository
   git clone https://github.com/your-org/SauronVisionProtocol.git
   
   # Install .NET 9 SDK (if not already installed)
   # Instructions will be provided in setup documentation
   
   # Configure Azure credentials
   az login
   
   # Local development may not have all runtimes installed
   # Focus will be on GitHub Actions deployment pipeline for testing
   
   # For local testing with Docker (if available):
   docker build -t svp-server:dev .
   docker run -p 9000:9000 svp-server:dev
   ```

2. **Development Cycle**:
   - Feature development on feature branches
   - Pull requests for code review
   - GitHub Actions CI/CD pipeline for automated build, test and deployment
   - Automated testing on AKS environment

## Technical Constraints

1. **Azure Limitations**:
   - TCP socket handling in serverless environments may require specific approaches
   - Cold start considerations for serverless functions
   - Service limits and quotas to be monitored

2. **Cross-Platform Compatibility**:
   - UI must be consistent across macOS and Windows
   - Networking code must handle platform-specific nuances
   - Installation process needs to be streamlined for both platforms

3. **Protocol Design**:
   - Text-based for readability and debugging
   - Efficient encoding for performance
   - Extensible for future command additions

## Dependencies

### Server Dependencies

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <InvariantGlobalization>true</InvariantGlobalization>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
  </ItemGroup>
</Project>
```

### Client Dependencies

To be determined based on the selected client framework.

*Note*: Actual dependencies will be finalized once implementation approach is decided.

## Tool Usage Patterns

### Version Control

- **Branch Strategy**:
  - `main`: Production-ready code
  - `develop`: Integration branch for features
  - `feature/feature-name`: Individual feature development
  - `release/vX.Y.Z`: Release preparation
  - `hotfix/issue-description`: Emergency fixes

- **Commit Conventions**:
  - Semantic commit messages: 
    - `feat: add new command type`
    - `fix: resolve connection timeout issue`
    - `docs: update Azure setup instructions`
    - `refactor: improve command processor`
    - `test: add unit tests for response parser`

### Code Quality Tools

- **Linting**: ESLint with project-specific rule sets for consistent code style
- **Formatting**: Prettier for automated code formatting
- **Testing**: Jest for unit and integration tests
- **Documentation**: JSDoc for code documentation, Markdown for general documentation

### Deployment

- **Environments**:
  - Development: For active development and testing on AKS
  - Staging: Pre-production verification on AKS
  - Production: Live environment on AKS

- **Release Process**:
  - Version bump according to semantic versioning
  - GitHub Actions workflow triggered by tags or pushes to specific branches
  - Container image built and tagged with version
  - Image pushed to Azure Container Registry
  - Kubernetes manifests applied to update deployment in AKS
  - Automated validation tests run against the deployed service
  - Promotion between environments through Git workflow

### Monitoring

- **Logging Strategy**:
  - Structured JSON logs
  - Different log levels (DEBUG, INFO, WARN, ERROR)
  - Centralized log collection in Azure

- **Performance Tracking**:
  - Connection metrics
  - Command processing time
  - Error rates
  - Client-side responsiveness

## Development Standards

1. **Code Style**:
   - Consistent naming conventions
   - Modular, testable components
   - Comprehensive error handling
   - Comments explaining "why" not "what"

2. **Documentation**:
   - README.md in each directory explaining purpose
   - API documentation for protocol
   - Setup guides for different environments
   - Troubleshooting guides

3. **Testing**:
   - Unit tests for individual components
   - Integration tests for component interaction
   - End-to-end tests for complete flows
   - Performance tests for critical paths
