# Pictyping Terraform Infrastructure

このディレクトリには、Pictyping ASP.NETプロジェクトをLinodeにデプロイするためのTerraform構成が含まれています。

## 概要

- **プロバイダー**: Linode
- **推奨インスタンス**: Nanode 1GB ($5/月)
- **自動化**: GitHub Actionsによる自動デプロイ

## 構成内容

### リソース

1. **Linodeインスタンス** (g6-nanode-1)
   - Ubuntu 22.04 LTS
   - Docker & Docker Compose
   - 自動セットアップスクリプト

2. **Linodeボリューム** (10GB)
   - PostgreSQLデータ
   - Redisデータ
   - SSL証明書

3. **ファイアウォール**
   - SSH (22)
   - HTTP (80)
   - HTTPS (443)

4. **StackScript**
   - Dockerインストール
   - アプリケーションデプロイ
   - systemdサービス設定

## 使用方法

### 1. 前提条件

- Terraform >= 1.0
- Linode APIトークン
- SSH公開鍵
- GitHub Personal Access Token（プライベートリポジトリの場合）

### 2. 環境変数の設定

```bash
export TF_VAR_linode_token="your-linode-api-token"
export TF_VAR_ssh_public_key="ssh-rsa AAAAB3..."
export TF_VAR_github_token="ghp_..."
export TF_VAR_jwt_key="your-secret-jwt-key"
export TF_VAR_google_client_id="your-google-client-id"
export TF_VAR_google_client_secret="your-google-client-secret"
export TF_VAR_db_password="secure-database-password"
export TF_VAR_redis_password="secure-redis-password"
```

### 3. Terraformの実行

```bash
# 初期化
terraform init

# プランの確認
terraform plan -var-file=environments/sandbox.tfvars

# デプロイ
terraform apply -var-file=environments/sandbox.tfvars

# 削除
terraform destroy -var-file=environments/sandbox.tfvars
```

## GitHub Actionsでの自動デプロイ

### 必要なシークレット

以下のシークレットをGitHubリポジトリに設定してください：

**実際のデプロイに必要:**
- `LINODE_TOKEN`: Linode APIトークン（必須）
- `SSH_PUBLIC_KEY`: SSH公開鍵（必須）
- `JWT_KEY`: JWT署名キー（必須）
- `GOOGLE_CLIENT_ID`: Google OAuth Client ID（必須）
- `GOOGLE_CLIENT_SECRET`: Google OAuth Client Secret（必須）
- `DB_PASSWORD`: PostgreSQLパスワード（必須）
- `REDIS_PASSWORD`: Redisパスワード（必須）

**オプション:**
- `GH_TOKEN`: GitHub Personal Access Token（デフォルトで`github.token`を使用）

**Terraform Plan（PR時）の場合:**
シークレットが設定されていない場合、ダミー値で構文チェックのみ実行されます。

### バックエンドストレージ（オプション）

Terraform stateを管理する場合：

- `TF_STATE_BUCKET`: S3互換バケット名
- `AWS_ACCESS_KEY_ID`: アクセスキー
- `AWS_SECRET_ACCESS_KEY`: シークレットキー
- `AWS_REGION`: リージョン

### ワークフローの使用

1. **手動デプロイ**: Actions → Deploy Sandbox Environment → Run workflow
2. **自動プラン**: PRを作成すると自動的にterraform planが実行されます

## コスト見積もり

月額コスト（概算）：
- Linodeインスタンス (Nanode 1GB): $5
- Linodeボリューム (10GB): $1
- **合計**: 約$6/月

## セキュリティ考慮事項

1. すべての機密情報は環境変数またはGitHub Secretsで管理
2. ファイアウォールで必要なポートのみ開放
3. 自動セキュリティアップデート有効化
4. HTTPS/SSL対応（Let's Encrypt使用可能）

## トラブルシューティング

### インスタンスにSSH接続

```bash
ssh root@$(terraform output -raw instance_ip)
```

### ログの確認

```bash
# Dockerコンテナのログ
docker-compose logs -f

# システムログ
journalctl -u pictyping -f
```

### サービスの再起動

```bash
systemctl restart pictyping
```