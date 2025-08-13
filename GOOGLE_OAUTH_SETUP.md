# Google OAuth Setup Guide

## 概要
このドキュメントは、Pictyping .NET版でGoogle OAuth認証を設定・使用するためのガイドです。

## 開発環境のアクセスポイント

- フロントエンド: http://localhost:3001
- API: http://localhost:5001
- Swagger: http://localhost:5001/swagger
- Adminer (DB UI): http://localhost:8081
- Nginx: http://localhost:8001

## Google OAuth認証フロー

1. **ユーザーがログインページにアクセス**
   - URL: http://localhost:3001/login
   - 「Googleでログイン」ボタンが表示されます

2. **Googleログインボタンをクリック**
   - `/api/auth/google/login` エンドポイントにリダイレクトされます
   - Googleの認証画面に転送されます

3. **Google認証後のコールバック**
   - `/api/auth/google/callback` でユーザー情報を処理
   - OmniAuthIdentityレコードを作成/更新
   - JWTトークンを生成

4. **フロントエンドへのリダイレクト**
   - `/auth/callback?token={jwt_token}` にリダイレクト
   - トークンをlocalStorageに保存
   - ユーザー情報を取得してReduxストアに保存

## Google OAuth設定

### 1. Google Cloud Console設定

1. [Google Cloud Console](https://console.cloud.google.com/)にアクセス
2. 新しいプロジェクトを作成するか、既存のプロジェクトを選択
3. 「APIとサービス」→「認証情報」に移動
4. 「認証情報を作成」→「OAuth クライアント ID」を選択
5. アプリケーションの種類: ウェブアプリケーション
6. 承認済みのリダイレクトURI:
   - 開発環境: `http://localhost:5001/api/auth/google/callback`
   - 本番環境: `https://new.pictyping.com/api/auth/google/callback`

### 2. アプリケーション設定

`appsettings.json` または環境変数で設定:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

Docker環境変数での設定:
```yaml
environment:
  - Authentication__Google__ClientId=your-google-client-id.apps.googleusercontent.com
  - Authentication__Google__ClientSecret=your-google-client-secret
```

## データベーススキーマ

### OmniAuthIdentityテーブル
- `provider`: OAuth プロバイダー名（"google"）
- `uid`: GoogleのユーザーID
- `user_id`: 関連するユーザーのID

### Usersテーブル
- 新規OAuthユーザーは自動的に作成されます
- `email`: Googleアカウントのメールアドレス
- `name`: Googleアカウントの表示名（またはメールのローカル部分）
- `encrypted_password`: 空文字列（OAuthユーザーはパスワードなし）

## トラブルシューティング

### ポート競合エラー
既存のコンテナが動いている場合は、`docker-compose.development.yml`でポートを変更してください。

### Google認証エラー
- ClientIdとClientSecretが正しく設定されているか確認
- リダイレクトURIがGoogle Cloud Consoleで設定されているか確認
- HTTPSが必要な場合は、開発環境でもHTTPSを有効にする

### セッション管理
- Redis接続を確認: `docker logs pictyping-oauth-feature-redis-1`
- トークンの有効期限はデフォルトで60分

## セキュリティ考慮事項

1. **本番環境では必ずHTTPSを使用**
2. **ClientSecretは環境変数で管理し、コードにハードコードしない**
3. **JWTトークンのキーは強力なものを使用**
4. **CORSポリシーを適切に設定**

## 今後の改善点

- [ ] リフレッシュトークンの実装
- [ ] ソーシャルログインプロバイダーの追加（Facebook、Twitter等）
- [ ] ユーザープロフィール画像の取得と保存
- [ ] 2要素認証のサポート