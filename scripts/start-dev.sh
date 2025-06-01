#!/bin/bash

# 開発環境起動スクリプト

echo "Starting Pictyping development environment..."

# .env.development ファイルが存在するかチェック
if [ ! -f .env.development ]; then
    echo "Error: .env.development file not found!"
    echo "Please copy .env.development.example to .env.development and configure it."
    exit 1
fi

# 環境変数を読み込む
export $(cat .env.development | grep -v '^#' | xargs)

# Docker Composeで開発環境を起動
docker-compose -f docker-compose.development.yml up --build

echo "Development environment is starting..."
echo "API will be available at: http://localhost:5000"
echo "Web UI will be available at: http://localhost:3000"
echo "Adminer (DB UI) will be available at: http://localhost:8080"