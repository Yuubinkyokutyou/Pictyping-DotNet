# Pictyping Rails to ASP.NET Core + React 移行ガイド

## 概要
このドキュメントは、Pictyping RailsアプリケーションをASP.NET Core + Reactに移行するための詳細な手順書です。

## 前提条件
- .NET 8.0 SDK
- Node.js 18以上
- Docker Desktop
- PostgreSQL (既存のデータベースを使用)
- Redis (既存のインスタンスを使用)

## アーキテクチャ概要

### 新システム構成
```
Pictyping-DotNet/
├── src/
│   ├── Pictyping.API/          # ASP.NET Core Web API
│   ├── Pictyping.Web/          # React SPA
│   ├── Pictyping.Core/         # ビジネスロジック・エンティティ
│   └── Pictyping.Infrastructure/ # データアクセス・外部サービス
├── docs/                       # ドキュメント
├── scripts/                    # 移行・デプロイスクリプト
└── tests/                      # テストプロジェクト
```

## 移行手順

### Phase 1: 開発環境のセットアップ

#### 1.1 ASP.NET Core APIプロジェクトの作成
```bash
cd src/Pictyping.API
dotnet new webapi -n Pictyping.API
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package StackExchange.Redis
```

#### 1.2 React プロジェクトの作成
```bash
cd src/Pictyping.Web
npm create vite@latest . -- --template react-ts
npm install axios react-router-dom @reduxjs/toolkit react-redux
```

### Phase 2: データベース移行

#### 2.1 Entity Framework Coreモデルの生成
既存のPostgreSQLデータベースからリバースエンジニアリング：
```bash
cd src/Pictyping.Infrastructure
dotnet ef dbcontext scaffold "Host=localhost;Database=pictyping;Username=postgres;Password=password" Npgsql.EntityFrameworkCore.PostgreSQL -o Data/Models --context PictypingDbContext
```

#### 2.2 データベースアクセスの設定
既存のスキーマをそのまま使用し、テーブル名・カラム名のマッピングを調整。

### Phase 3: 認証システムの実装

#### 3.1 ドメイン間認証の仕組み
新旧システム間でシームレスな認証を実現するため、以下の方式を採用：

1. **共有認証トークン**: JWT形式で両システムで検証可能
2. **リダイレクトフロー**: ドメイン間でトークンを安全に受け渡し
3. **セッション同期**: Redisを使用して両システムでセッション情報を共有

#### 3.2 実装詳細
- 旧システム（Rails）: JWT発行機能を追加
- 新システム（ASP.NET）: JWT検証とセッション管理
- 両システム共通: 同じRedisインスタンスを使用

### Phase 4: API実装

#### 4.1 主要APIエンドポイント
以下のエンドポイントを新システムに実装：
- `/api/users` - ユーザー管理
- `/api/typing-battles` - タイピング対戦
- `/api/rankings` - ランキング
- `/api/penalties` - ペナルティ管理

#### 4.2 API互換性
既存のRails APIと同じレスポンス形式を維持し、クライアント側の変更を最小限に。

### Phase 5: フロントエンド実装

#### 5.1 ページ構成
- ホームページ
- ユーザープロファイル
- ランキング
- 利用規約・プライバシーポリシー
- お問い合わせ

#### 5.2 Unity WebGL統合
Unity WebGLコンテンツは独立したプロジェクトとして扱い、iframeまたは直接埋め込みで統合。

### Phase 6: ドメイン移行

#### 6.1 DNS設定
- 新ドメイン: `new.pictyping.com` (例)
- 旧ドメイン: `pictyping.com`

#### 6.2 リダイレクト設定
両ドメイン間で相互にリダイレクトし、認証状態を維持。

### Phase 7: デプロイメント

#### 7.1 Docker構成
```yaml
version: '3.8'
services:
  api:
    build: ./src/Pictyping.API
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=pictyping
      - Redis__ConnectionString=redis:6379
  
  web:
    build: ./src/Pictyping.Web
    depends_on:
      - api
  
  nginx:
    image: nginx:alpine
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
    ports:
      - "80:80"
      - "443:443"
```

#### 7.2 本番環境への切り替え
1. 新システムを別サーバーにデプロイ
2. データベースレプリケーション設定
3. DNSのTTLを短縮
4. 段階的にトラフィックを新システムへ
5. 問題がなければ完全切り替え

## ロールバック計画

問題が発生した場合：
1. DNSを旧システムに戻す
2. データベースの変更をロールバック
3. ログを分析して問題を特定

## チェックリスト

- [ ] 開発環境構築完了
- [ ] データベースモデル生成完了
- [ ] 認証システム実装完了
- [ ] 全APIエンドポイント実装完了
- [ ] フロントエンド実装完了
- [ ] ドメイン間認証テスト完了
- [ ] 負荷テスト実施
- [ ] セキュリティ監査実施
- [ ] バックアップ確認
- [ ] ロールバック手順確認
- [ ] 本番デプロイ完了