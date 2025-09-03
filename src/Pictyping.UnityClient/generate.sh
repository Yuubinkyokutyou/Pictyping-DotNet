#!/bin/bash

# API Server URL
API_URL="http://localhost:5000/swagger/v1/swagger.json"

echo "Downloading latest swagger.json from API server..."

# Download swagger.json from API server with timeout and error handling
if curl --connect-timeout 10 --max-time 30 -f -s -o swagger.json "$API_URL"; then
    echo "✓ Successfully downloaded swagger.json"
else
    echo "✗ Error: Failed to download swagger.json from $API_URL"
    echo "Please make sure the API server is running on localhost:5000"
    exit 1
fi

echo "Generating Unity WebRequest C# API Client..."

# Generate Unity WebRequest C# API Client
# Set MSYS_NO_PATHCONV to prevent Git Bash from converting paths
export MSYS_NO_PATHCONV=1
docker run --rm \
  -v "$(pwd):/local" \
  openapitools/openapi-generator-cli:latest generate \
  -i /local/swagger.json \
  -g csharp \
  -o /local/generated \
  --additional-properties=library=unityWebRequest,packageName=Pictyping.UnityClient,packageVersion=1.0.0,targetFramework=netstandard2.1,packageCompany=Pictyping,packageTitle="Pictyping Unity API Client",packageDescription="Unity WebRequest-based API client for Pictyping",generatePropertyChanged=false,nullableReferenceTypes=false,netCoreProjectFile=true,sourceFolder=src

echo "✓ Unity API Client generation completed!"