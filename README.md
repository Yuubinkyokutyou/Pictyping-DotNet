# Pictyping DotNet

[![CI/CD Pipeline](https://github.com/Yuubinkyokutyou/Pictyping-DotNet/actions/workflows/ci.yml/badge.svg)](https://github.com/Yuubinkyokutyou/Pictyping-DotNet/actions/workflows/ci.yml)
[![Code Coverage](https://github.com/Yuubinkyokutyou/Pictyping-DotNet/actions/workflows/coverage.yml/badge.svg)](https://github.com/Yuubinkyokutyou/Pictyping-DotNet/actions/workflows/coverage.yml)
[![Dependency Check](https://github.com/Yuubinkyokutyou/Pictyping-DotNet/actions/workflows/dependency-check.yml/badge.svg)](https://github.com/Yuubinkyokutyou/Pictyping-DotNet/actions/workflows/dependency-check.yml)

Pictyping RailsアプリケーションのASP.NET Core + React版

## プロジェクト構成

```
Pictyping-DotNet/
├── src/
│   ├── Pictyping.API/          # ASP.NET Core Web API
│   ├── Pictyping.Web/          # React フロントエンド
│   ├── Pictyping.Core/         # ドメインモデル・ビジネスロジック
│   └── Pictyping.Infrastructure/ # データアクセス・外部サービス連携
├── docs/                       # ドキュメント
├── scripts/                    # 移行・運用スクリプト
└── tests/                      # テストプロジェクト
```

## セットアップ

### 前提条件

- Docker Desktop
- .NET 8.0 SDK (ローカル開発の場合)
- Node.js 18+ (ローカル開発の場合)

### 環境変数の設定

1. `.env.example` を `.env` にコピー
```bash
cp .env.example .env
```

2. `.env` ファイルを編集して必要な値を設定
- `POSTGRES_PASSWORD`: 既存のRailsプロジェクトと同じ値
- `JWT_SECRET_KEY`: 32文字以上のランダムな文字列
- `GOOGLE_CLIENT_ID/SECRET`: Google OAuth認証用

### 初回起動

```bash
# 開発環境の起動（ホットリロード対応）
chmod +x scripts/start-dev.sh
./scripts/start-dev.sh

# 本番環境の起動
chmod +x scripts/start-prod.sh
./scripts/start-prod.sh

# 別のターミナルでマイグレーション実行
chmod +x scripts/migration/migrate-database.sh
./scripts/migration/migrate-database.sh
```

### 環境別Docker Compose

- **開発環境** (`docker-compose.development.yml`):
  - ホットリロード対応
  - ボリュームマウントによるコード変更の即時反映
  - Adminer (データベース管理UI) 付属
  - デバッグモード有効

- **本番環境** (`docker-compose.production.yml`):
  - 最適化されたビルド
  - ヘルスチェック設定
  - 自動再起動設定
  - セキュリティ強化

## テスト

### テスト実行

```bash
# 全テスト実行
dotnet test Pictyping.sln

# カバレッジ付きテスト実行
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# 単体テストのみ実行
dotnet test tests/Pictyping.Core.Tests/

# 統合テストのみ実行
dotnet test tests/Pictyping.API.Tests/
```

### テスト構成

- **単体テスト**: 27テスト (Core層のエンティティ・サービス)
- **統合テスト**: 22テスト (API エンドポイント)
- **カバレッジ**: 自動生成される詳細レポート
- **CI/CD**: GitHub Actions で自動実行

## 開発

### ローカル開発

```bash
# API開発
cd src/Pictyping.API
dotnet watch run

# フロントエンド開発
cd src/Pictyping.Web
npm install
npm run dev
```

### APIクライアントの自動生成

フロントエンドでは、Swagger/OpenAPIからTypeScriptのAPIクライアントを自動生成できます：

```bash
cd src/Pictyping.Web

# APIクライアントの生成（swagger.jsonから）
npm run generate-api

# APIの変更を監視して自動生成
npm run generate-api:watch
```

生成されたAPIクライアントは `src/api/generated/` に出力され、以下のように使用できます：

```typescript
import { authApi, rankingApi } from '../api/apiConfig'

// ログイン
const response = await authApi.login({ 
  requestBody: { email, password } 
})

// ランキング取得
const rankings = await rankingApi.getRankings({ 
  page: 1, 
  pageSize: 20 
})
```

### Docker環境での開発

```bash
# 全サービス起動
docker-compose up

# APIのみ再起動
docker-compose restart api

# ログ確認
docker-compose logs -f api
docker-compose logs -f web
```

## アクセスURL

### 開発環境
- フロントエンド: http://localhost:3000
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- Adminer (DB管理): http://localhost:8080

### 本番環境
- Webアプリケーション: http://localhost:80
- API: http://localhost:5000

## ドメイン間認証のテスト

1. `/etc/hosts` に以下を追加（ローカルテスト用）:
```
127.0.0.1 pictyping.com
127.0.0.1 new.pictyping.com
```

2. 旧システム（Rails）を起動
3. 新システム（.NET）を起動
4. 各ドメインでログインし、相互にアクセスできることを確認

## デプロイ

### 本番環境へのデプロイ

1. 本番用の環境変数を設定
2. SSL証明書を配置
3. Docker イメージをビルドしてプッシュ
```bash
docker build -f src/Pictyping.API/Dockerfile -t pictyping-api:latest .
docker build -f src/Pictyping.Web/Dockerfile -t pictyping-web:latest .
```

## トラブルシューティング

### データベース接続エラー
- PostgreSQLコンテナが起動しているか確認
- `.env` の `POSTGRES_PASSWORD` が正しいか確認

### 認証エラー
- JWT_SECRET_KEY が両システムで一致しているか確認
- Redisが正しく動作しているか確認

### CORS エラー
- `appsettings.json` の CORS 設定を確認
- nginx の設定を確認

## 詳細ドキュメント

- [移行ガイド](docs/MIGRATION_GUIDE.md)
- [Rails統合設定](docs/RAILS_INTEGRATION.md)