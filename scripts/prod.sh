#!/bin/bash

echo "Starting Pictyping in production mode..."

# Load environment variables if .env file exists
if [ -f .env ]; then
  export $(cat .env | xargs)
fi

# Use production compose file
docker-compose -f docker-compose.prod.yml up -d