# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/*.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY ["src/", "./"]
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

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
