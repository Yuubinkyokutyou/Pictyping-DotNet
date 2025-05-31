#!/bin/bash

echo "=== Pictyping Database Migration Script ==="
echo "This script will configure the new .NET system to use the existing Rails database"
echo ""

# 環境変数チェック
if [ ! -f .env ]; then
    echo "Error: .env file not found. Please copy .env.example to .env and configure it."
    exit 1
fi

# 環境変数を読み込む
export $(cat .env | grep -v '^#' | xargs)

echo "1. Checking database connection..."
docker-compose run --rm api dotnet ef database scaffold \
    "Host=db;Database=pictyping;Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}" \
    Npgsql.EntityFrameworkCore.PostgreSQL \
    -o Data/GeneratedModels \
    --context-dir Data \
    --context GeneratedDbContext \
    --force

if [ $? -eq 0 ]; then
    echo "✓ Database connection successful"
    echo "✓ Entity models generated from existing database"
else
    echo "✗ Database connection failed"
    exit 1
fi

echo ""
echo "2. Running data validation..."
docker-compose run --rm api dotnet run --project scripts/ValidateData

echo ""
echo "3. Setting up Redis connection..."
docker-compose run --rm api dotnet run --project scripts/TestRedisConnection

echo ""
echo "=== Migration preparation complete ==="
echo ""
echo "Next steps:"
echo "1. Review the generated models in src/Pictyping.Infrastructure/Data/GeneratedModels"
echo "2. Run 'docker-compose up' to start the new system"
echo "3. Access the new system at http://localhost:3000"
echo "4. Test cross-domain authentication between old and new systems"