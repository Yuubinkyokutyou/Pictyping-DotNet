@echo off
chcp 65001 >nul 2>&1

REM Generate Unity WebRequest C# API Client for Windows
echo ========================================
echo Unity API Client Generator
echo ========================================

REM Configuration
set "API_URL=http://localhost:5000/swagger/v1/swagger.json"
set "CURRENT_DIR=%cd%"

echo Checking prerequisites...

REM Check if Docker is available
docker --version >nul 2>&1
if errorlevel 1 (
    echo Error: Docker is not installed or not in PATH
    echo Please install Docker Desktop and make sure it's running
    echo https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

REM Check if Docker daemon is running
docker info >nul 2>&1
if errorlevel 1 (
    echo Error: Docker daemon is not running
    echo Please start Docker Desktop and try again
    pause
    exit /b 1
)

echo Docker is available and running

echo.
echo Downloading latest swagger.json from API server...

REM Clean up any existing swagger.json
if exist swagger.json del /q swagger.json

REM Download swagger.json from API server with error handling
curl --connect-timeout 10 --max-time 30 -f -s -o swagger.json "%API_URL%"
if errorlevel 1 (
    echo Error: Failed to download swagger.json from %API_URL%
    echo.
    echo Troubleshooting tips:
    echo 1. Make sure the API server is running on localhost:5000
    echo 2. Check if you can access %API_URL% in your browser
    echo 3. Verify the server is serving the swagger endpoint
    echo.
    pause
    exit /b 1
)

REM Verify the downloaded file exists
if not exist swagger.json (
    echo Error: swagger.json was not created
    pause
    exit /b 1
)

echo Successfully downloaded swagger.json

echo.
echo Generating Unity WebRequest C# API Client...

REM Clean up existing generated folder
if exist generated (
    echo Cleaning up existing generated files...
    rmdir /s /q generated 2>nul
)

REM Generate Unity WebRequest C# API Client using Docker via PowerShell
powershell -Command "docker run --rm -v \"${PWD}:/local\" openapitools/openapi-generator-cli:latest generate -i /local/swagger.json -g csharp -o /local/generated --additional-properties=library=unityWebRequest,packageName=Pictyping.UnityClient,packageVersion=1.0.0,targetFramework=netstandard2.1,packageCompany=Pictyping,packageTitle=\"Pictyping Unity API Client\",packageDescription=\"Unity WebRequest-based API client for Pictyping\",generatePropertyChanged=false,nullableReferenceTypes=false,netCoreProjectFile=true,sourceFolder=src"

if errorlevel 1 (
    echo Error: OpenAPI Generator failed
    echo.
    echo Troubleshooting tips:
    echo 1. Check if Docker has enough resources allocated
    echo 2. Verify the swagger.json file is valid OpenAPI specification
    echo 3. Try running Docker commands manually to test connectivity
    echo.
    pause
    exit /b 1
)

REM Verify generation was successful
if not exist generated (
    echo Error: Generated folder was not created
    pause
    exit /b 1
)

if not exist generated\src (
    echo Error: Generated source folder was not created
    pause
    exit /b 1
)

echo Unity API Client generation completed successfully!

REM Show summary
echo.
echo ========================================
echo Generation Summary:
echo ========================================
echo Source location: %CURRENT_DIR%\generated\src
echo Package name: Pictyping.UnityClient
echo Target framework: netstandard2.1
echo Library type: Unity WebRequest
echo ========================================

REM Keep swagger.json for debugging/reference purposes

echo.
echo All done! You can now use the generated Unity client.
echo Check UNITY_INTEGRATION.md for integration instructions.

pause