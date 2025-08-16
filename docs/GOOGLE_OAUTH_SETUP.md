# Google OAuth 設定ガイド

## 概要
PictypingアプリケーションでGoogle OAuthログインを有効にするための設定手順です。

## 前提条件
- Google Cloud Platformアカウント
- プロジェクトの管理者権限

## 設定手順

### 1. Google Cloud Console でのプロジェクト設定

#### 1.1 プロジェクトの作成または選択
1. [Google Cloud Console](https://console.cloud.google.com/)にアクセス
2. 新しいプロジェクトを作成するか、既存のプロジェクトを選択

#### 1.2 OAuth 2.0認証情報の作成
1. 左メニューから「APIとサービス」→「認証情報」を選択
2. 「認証情報を作成」→「OAuth クライアント ID」をクリック
3. アプリケーションの種類として「ウェブアプリケーション」を選択
4. 名前を入力（例：「Pictyping Web Client」）

#### 1.3 承認済みリダイレクトURIの設定
以下のURIを追加してください：

**開発環境:**
```
http://localhost:5000/api/auth/google/callback
http://localhost:3000/api/auth/google/callback
```

**本番環境:**
```
https://pictyping.com/api/auth/google/callback
https://new.pictyping.com/api/auth/google/callback
```

#### 1.4 OAuth同意画面の設定
1. 「OAuth同意画面」タブを選択
2. アプリケーション名、サポートメール、ロゴなどを設定
3. スコープとして以下を追加：
   - `email`
   - `profile`
   - `openid`

### 2. アプリケーション設定

#### 2.1 環境変数の設定
`.env`ファイルまたは`appsettings.json`に以下を設定：

**開発環境 (appsettings.Development.json):**
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

**環境変数による設定:**
```bash
# .env ファイル
GOOGLE_CLIENT_ID=YOUR_GOOGLE_CLIENT_ID
GOOGLE_CLIENT_SECRET=YOUR_GOOGLE_CLIENT_SECRET
```

#### 2.2 Dockerコンテナでの設定
`docker-compose.yml`に環境変数を追加：

```yaml
services:
  api:
    environment:
      - Authentication__Google__ClientId=${GOOGLE_CLIENT_ID}
      - Authentication__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
```

### 3. トラブルシューティング

#### 404エラーが発生する場合

##### 原因1: リダイレクトURIの不一致
- Google Cloud ConsoleのリダイレクトURIと実際のコールバックURLが完全に一致していることを確認
- プロトコル（http/https）、ポート番号、パスすべてが一致している必要があります

##### 原因2: APIが有効になっていない
1. Google Cloud Consoleで「APIとサービス」→「有効なAPI」を確認
2. 「Google+ API」または「Google Identity Platform」が有効になっていることを確認

##### 原因3: CORSの設定
API側でCORSが正しく設定されていることを確認：

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000", "https://pictyping.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

#### 認証エラーが発生する場合

##### 原因1: クライアントIDまたはシークレットの誤り
- 環境変数やappsettings.jsonの値が正しいことを確認
- 値の前後に余分なスペースがないことを確認

##### 原因2: OAuth同意画面の設定不足
- 必要なスコープが追加されていることを確認
- テストユーザーが追加されていることを確認（開発環境の場合）

### 4. 実装確認

#### APIエンドポイント
以下のエンドポイントが正しく動作することを確認：

- `GET /api/auth/google` - Google認証画面へのリダイレクト
- `GET /api/auth/google/callback` - 認証後のコールバック処理

#### フロントエンド
Reactコンポーネントで以下が実装されていることを確認：

```typescript
// Google ログインボタンの実装例
const handleGoogleLogin = () => {
  window.location.href = `${API_BASE_URL}/api/auth/google`;
};
```

### 5. セキュリティの考慮事項

- **本番環境では必ずHTTPSを使用**
- クライアントシークレットは環境変数で管理し、ソースコードにハードコードしない
- 定期的にクライアントシークレットをローテーション
- 不要なスコープは要求しない

### 6. 参考リンク

- [Google OAuth 2.0 公式ドキュメント](https://developers.google.com/identity/protocols/oauth2)
- [ASP.NET Core での Google 認証](https://docs.microsoft.com/aspnet/core/security/authentication/social/google-logins)
- [Google Cloud Console](https://console.cloud.google.com/)