# Pictyping Unity Client

Unity WebRequest用のC# APIクライアントです。openapi-generator-cliを使用して生成されています。

## クライアント生成方法

### 前提条件
- Dockerがインストールされていること
- APIサーバーが localhost:5000 で動作していること（swagger.json自動取得のため）

### コマンド

#### 推奨方法（自動swagger.json更新付き）
スクリプトを実行すると、APIサーバーから最新のswagger.jsonを自動ダウンロードしてからクライアントを生成します。

**Windows環境**
```bash
cd src/Pictyping.UnityClient
generate.bat
```

**Linux/Mac環境**
```bash
cd src/Pictyping.UnityClient
./generate.sh
```

#### npm経由での生成
```bash
cd src/Pictyping.Web
npm run generate-api:csharp:unity
```

### swagger.json自動更新について

generate.sh と generate.bat は実行時に自動的に以下を行います：

1. **APIサーバーから最新のswagger.jsonをダウンロード** (`http://localhost:5000/swagger/v1/swagger.json`)
2. **エラーハンドリング**: APIサーバーが停止している場合はエラーメッセージを表示して終了
3. **Unity APIクライアントの生成**: ダウンロード成功後にコード生成を実行

これにより、常に最新のAPI仕様に基づいたUnity Clientが生成されます。

## Unityプロジェクトへの統合

**手動配置方法（推奨）**

1. `Runtime/` フォルダをUnityプロジェクトの `Assets/` フォルダ内に `Pictyping.UnityClient` としてコピー
2. Unity Package Manager で `com.unity.nuget.newtonsoft-json` をインストール
3. 必要に応じてasmdefファイルを編集

## ディレクトリ構造

```
src/Pictyping.UnityClient/
├── Runtime/                     # Unity Asset配置用フォルダ
│   ├── Api/                     # APIクライアント
│   ├── Client/                  # HTTPクライアント実装
│   ├── Model/                   # データモデル
│   └── Pictyping.UnityClient.asmdef
├── swagger.json                 # OpenAPI仕様ファイル
├── openapi-config.json          # 生成設定
├── generate.sh                  # Linux/Mac用生成スクリプト
└── generate.bat                 # Windows用生成スクリプト
```

## APIクラス

- **AuthApi**: 認証関連のAPI
- **RankingApi**: ランキング関連のAPI
- **PictypingAPIApi**: その他のAPI

## 注意事項

生成されたコードはUnityWebRequestを使用するため、Unity環境でのみ動作します。通常の.NETプロジェクトでのビルドにはUnityEngineへの参照が必要です。