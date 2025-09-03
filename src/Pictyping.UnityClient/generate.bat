@echo off
REM Generate Unity WebRequest C# API Client for Windows

set CURRENT_DIR=%cd%
set DOCKER_PATH=/c/Users/oonni/.sandbox/_Pictyping/workspace/src/Pictyping.UnityClient

docker run --rm ^
  -v "%CURRENT_DIR%:/local" ^
  openapitools/openapi-generator-cli:latest generate ^
  -i /local/swagger.json ^
  -g csharp ^
  -o /local/generated ^
  --additional-properties=library=unityWebRequest,packageName=Pictyping.UnityClient,packageVersion=1.0.0,targetFramework=netstandard2.1,packageCompany=Pictyping,packageTitle="Pictyping Unity API Client",packageDescription="Unity WebRequest-based API client for Pictyping",generatePropertyChanged=false,nullableReferenceTypes=false,netCoreProjectFile=true,sourceFolder=src