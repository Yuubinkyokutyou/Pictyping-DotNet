#!/bin/bash

# 本番環境起動スクリプト

echo "Starting Pictyping production environment..."

# .env.production ファイルが存在するかチェック
if [ ! -f .env.production ]; then
    echo "Error: .env.production file not found!"
    echo "Please create .env.production based on .env.production.example"
    exit 1
fi

# 環境変数を読み込む
export $(cat .env.production | grep -v '^#' | xargs)

# Docker Composeで本番環境を起動
docker-compose -f docker-compose.production.yml up -d

echo "Production environment is starting..."
echo "Application will be available at: http://localhost:80"