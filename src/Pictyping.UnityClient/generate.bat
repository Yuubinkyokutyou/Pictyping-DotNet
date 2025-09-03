@echo off
REM Generate Unity WebRequest C# API Client for Windows

set CURRENT_DIR=%cd%
set API_URL=http://localhost:5000/swagger/v1/swagger.json

echo Downloading latest swagger.json from API server...

REM Download swagger.json from API server with error handling
curl --connect-timeout 10 --max-time 30 -f -s -o swagger.json "%API_URL%"
if %ERRORLEVEL% neq 0 (
    echo × Error: Failed to download swagger.json from %API_URL%
    echo Please make sure the API server is running on localhost:5000
    exit /b 1
)

echo ✓ Successfully downloaded swagger.json
echo Generating Unity WebRequest C# API Client...

docker run --rm ^
  -v "%CURRENT_DIR%:/local" ^
  openapitools/openapi-generator-cli:latest generate ^
  -i /local/swagger.json ^
  -g csharp ^
  -o /local/generated ^
  --additional-properties=library=unityWebRequest,packageName=Pictyping.UnityClient,packageVersion=1.0.0,targetFramework=netstandard2.1,packageCompany=Pictyping,packageTitle="Pictyping Unity API Client",packageDescription="Unity WebRequest-based API client for Pictyping",generatePropertyChanged=false,nullableReferenceTypes=false,netCoreProjectFile=true,sourceFolder=src

echo ✓ Unity API Client generation completed!