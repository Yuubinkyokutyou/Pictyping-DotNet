# OAuth Email Verification Security Fix

## Issue
OAuth認証でメールアドレスの検証が不十分で、未検証のメールアドレスでも認証が通る可能性がありました。

## Root Cause
Google OAuth認証において以下の問題がありました：
1. `email_verified`クレームの確認が不足
2. OmniAuthIdentityエンティティが使用されていない
3. OAuthプロバイダーのUIDが記録されていない

## Security Improvements

### 1. Email Verification Check
GoogleCallbackメソッドで`email_verified`クレームをチェックし、Googleが検証していないメールアドレスでの認証を拒否します。

```csharp
var emailVerifiedClaim = authenticateResult.Principal?.FindFirst("email_verified")?.Value;
if (emailVerifiedClaim != "true")
{
    _logger.LogWarning("Google OAuth: Email not verified for {Email}", email);
    return BadRequest("Email not verified by Google");
}
```

### 2. OAuth Identity Management
`FindOrCreateUserByOAuthAsync`メソッドを追加し、適切なOAuth識別子の管理を実装：
- プロバイダーUID（Google UIDなど）の記録
- 既存ユーザーとOAuth認証の適切なリンク
- 重複アカウントの防止

### 3. Security Logging
認証失敗時のセキュリティログと成功時の監査ログを追加：
- 未検証メールアドレスの認証試行
- 認証成功時のユーザー情報

## Testing
OAuth認証に関する包括的なテストを追加：
- 新しいOAuthユーザーの作成
- 既存OAuthアイデンティティでの認証
- 既存ユーザーとOAuth認証のリンク
- メールアドレス変更時の更新

## Impact
この修正により、以下のセキュリティリスクが軽減されました：
- 未検証のメールアドレスによる不正アクセス
- アカウント乗っ取りのリスク
- 重複アカウントの問題