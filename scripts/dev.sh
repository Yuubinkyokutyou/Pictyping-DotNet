#!/bin/bash

echo "Starting Pictyping in development mode with hot reload..."
echo "- React changes will be reflected immediately"
echo "- C# changes will trigger automatic rebuild"

# Use development compose file
docker-compose -f docker-compose.dev.yml up --build