# Development stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /src

# Install dotnet-watch for hot reload
RUN dotnet tool install --global dotnet-watch

# Add dotnet tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Set environment for development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

# Expose ports for development
EXPOSE 5000
EXPOSE 5001

# Keep container running and use dotnet watch
CMD ["sleep", "infinity"]

# Base image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["src/Pictyping.API/Pictyping.API.csproj", "src/Pictyping.API/"]
COPY ["src/Pictyping.Core/Pictyping.Core.csproj", "src/Pictyping.Core/"]
COPY ["src/Pictyping.Infrastructure/Pictyping.Infrastructure.csproj", "src/Pictyping.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Pictyping.API/Pictyping.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/Pictyping.API"
RUN dotnet build "Pictyping.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Pictyping.API.csproj" -c Release -o /app/publish

# Production stage
FROM base AS production
WORKDIR /app
COPY --from=publish /app/publish .

# Configure ASP.NET Core to listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Pictyping.API.dll"]