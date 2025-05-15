# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["src/SauronVisionProtocol.Server.csproj", "src/"]
COPY ["shared/protocol/SauronVisionProtocol.Shared.csproj", "shared/protocol/"]
RUN dotnet restore "src/SauronVisionProtocol.Server.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "src/SauronVisionProtocol.Server.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/SauronVisionProtocol.Server.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the TCP port
EXPOSE 9000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:9000
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "SauronVisionProtocol.Server.dll"]
