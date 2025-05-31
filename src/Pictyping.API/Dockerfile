FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

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

FROM build AS publish
RUN dotnet publish "Pictyping.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configure ASP.NET Core to listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "Pictyping.API.dll"]