# Unity統合ガイド

このドキュメントでは、Pictyping.UnityClient をUnityプロジェクトに統合する方法を説明します。

## 概要

Pictyping.UnityClient は、Pictyping API サーバーとUnityアプリケーション間の通信を行うためのC# APIクライアントライブラリです。

- **UnityWebRequest** ベースの非同期通信
- **JWT認証** サポート
- **タイピングゲーム** に特化したAPI（認証、ランキング、レーティング）

## 前提条件

- Unity 2020.3 LTS 以上
- .NET Standard 2.1 サポート
- Newtonsoft.Json for Unity パッケージ

## インストール方法

### 手動配置方法（推奨）

1. **Runtime フォルダーをコピー**
   ```
   src/Pictyping.UnityClient/Runtime/
   ```
   を Unity プロジェクトの `Assets/` フォルダー内に `Pictyping.UnityClient` としてコピーします。

2. **Newtonsoft.Json for Unity をインストール**
   - Unity Package Manager で `com.unity.nuget.newtonsoft-json` を検索してインストール

3. **Assembly Definition の確認**
   - コピーした `Pictyping.UnityClient.asmdef` ファイルが存在することを確認
   - 必要に応じて依存関係を調整

## 基本的な使用方法

### 1. API クライアントの初期化

```csharp
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;

public class GameManager : MonoBehaviour
{
    private AuthApi authApi;
    private PictypingAPIApi gameApi;
    private RankingApi rankingApi;
    
    [SerializeField] private string apiBaseUrl = "http://localhost:5000";
    private string jwtToken = "";
    
    void Start()
    {
        InitializeAPIClients();
    }
    
    private void InitializeAPIClients()
    {
        var config = new Configuration();
        config.BasePath = apiBaseUrl;
        
        // JWT トークンが利用可能な場合は設定
        if (!string.IsNullOrEmpty(jwtToken))
        {
            config.AccessToken = jwtToken;
        }
        
        authApi = new AuthApi(config);
        gameApi = new PictypingAPIApi(config);
        rankingApi = new RankingApi(config);
    }
}
```

### 2. ログイン処理

```csharp
using System.Collections;
using Pictyping.UnityClient.Model;

public IEnumerator LoginCoroutine(string username, string password)
{
    var loginRequest = new LoginRequest
    {
        Username = username,
        Password = password
    };

    bool completed = false;
    string errorMessage = null;
    
    authApi.LoginAsync(loginRequest, (response, exception) =>
    {
        if (exception == null)
        {
            Debug.Log("ログイン成功");
            // レスポンスからJWTトークンを取得
            // jwtToken = response.Token; // APIレスポンスに応じて調整
        }
        else
        {
            errorMessage = exception.Message;
        }
        completed = true;
    });

    yield return new WaitUntil(() => completed);
    
    if (!string.IsNullOrEmpty(errorMessage))
    {
        Debug.LogError($"ログイン失敗: {errorMessage}");
    }
}
```

### 3. レーティング更新

```csharp
public IEnumerator UpdatePlayerRating(int newRating)
{
    var updateRequest = new UpdateRatingRequest
    {
        NewRating = newRating
    };

    bool completed = false;
    string errorMessage = null;

    gameApi.UpdateRatingAsync(updateRequest, (response, exception) =>
    {
        if (exception == null)
        {
            Debug.Log("レーティング更新成功");
        }
        else
        {
            errorMessage = exception.Message;
        }
        completed = true;
    });

    yield return new WaitUntil(() => completed);
    
    if (!string.IsNullOrEmpty(errorMessage))
    {
        Debug.LogError($"レーティング更新失敗: {errorMessage}");
    }
}
```

### 4. ランキング取得

```csharp
public IEnumerator GetRankingData()
{
    bool completed = false;
    string errorMessage = null;
    
    rankingApi.GetRankingAsync((response, exception) =>
    {
        if (exception == null)
        {
            Debug.Log("ランキング取得成功");
            // ランキングデータの処理
            ProcessRankingData(response);
        }
        else
        {
            errorMessage = exception.Message;
        }
        completed = true;
    });

    yield return new WaitUntil(() => completed);
    
    if (!string.IsNullOrEmpty(errorMessage))
    {
        Debug.LogError($"ランキング取得失敗: {errorMessage}");
    }
}
```

## 高度な使用方法

### カスタム設定

```csharp
private void InitializeAPIClients()
{
    var config = new Configuration();
    config.BasePath = apiBaseUrl;
    config.Timeout = 30000; // 30秒タイムアウト
    
    // カスタムヘッダー追加
    config.DefaultHeaders.Add("User-Agent", "Unity-Game/1.0");
    
    // JWT認証設定
    config.AccessToken = jwtToken;
    
    authApi = new AuthApi(config);
}
```

### エラーハンドリング

```csharp
private void HandleApiException(System.Exception exception)
{
    if (exception is ApiException apiException)
    {
        Debug.LogError($"API Error: {apiException.ErrorCode} - {apiException.Message}");
        
        switch (apiException.ErrorCode)
        {
            case 401:
                Debug.Log("認証が必要です");
                // ログイン画面に遷移
                break;
            case 403:
                Debug.Log("アクセス権限がありません");
                break;
            case 500:
                Debug.Log("サーバーエラーが発生しました");
                break;
            default:
                Debug.Log("予期しないエラーが発生しました");
                break;
        }
    }
    else
    {
        Debug.LogError($"Network Error: {exception.Message}");
    }
}
```

## トラブルシューティング

### よくある問題

1. **Newtonsoft.Json が見つからない**
   - Unity Package Manager で `com.unity.nuget.newtonsoft-json` をインストール

2. **UnityWebRequest関連のエラー**
   - Unity 2020.3 LTS 以上を使用していることを確認
   - `UnityEngine.Networking` 名前空間が利用可能であることを確認

3. **CORS エラー**
   - 開発環境では API サーバーで適切な CORS 設定が必要
   - プロダクション環境では同一オリジンからのアクセスを推奨

4. **JWT トークンの有効期限**
   - トークンの有効期限切れを適切にハンドリング
   - リフレッシュトークンの実装を検討

### デバッグ方法

```csharp
// デバッグログの有効化
#if UNITY_EDITOR
    Debug.unityLogger.logEnabled = true;
#endif

// API レスポンスのログ出力
authApi.LoginAsync(loginRequest, (response, exception) =>
{
    if (exception == null)
    {
        Debug.Log($"API Response: {response}");
    }
    else
    {
        Debug.LogError($"API Error: {exception}");
    }
});
```

## パフォーマンス最適化

- **コルーチンの適切な管理**: 不要になったコルーチンは確実に停止
- **API呼び出しの制限**: 短時間内での大量のAPI呼び出しを避ける
- **キャッシュの活用**: 頻繁にアクセスするデータのローカルキャッシュ

## セキュリティ考慮事項

- **JWT トークンの安全な保存**: PlayerPrefs ではなく、より安全な方法で保存
- **HTTPS の使用**: プロダクション環境では必ず HTTPS を使用
- **API キーの管理**: ソースコードに直接 API キーを埋め込まない

## サポート

問題が発生した場合は、以下を確認してください：

1. Unity コンソールのエラーログ
2. API サーバーのログ
3. ネットワーク接続状況
4. JWT トークンの有効性

## 関連ファイル

- `Samples~/BasicUsage/PictypingAPIExample.cs`: 基本的な使用例
- `generated/src/Pictyping.UnityClient/`: 生成されたAPIクライアント
- `Pictyping.UnityClient.asmdef`: Unity Assembly Definition