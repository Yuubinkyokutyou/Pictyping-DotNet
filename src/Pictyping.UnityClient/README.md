# Pictyping Unity Client

Unity WebRequest用のC# APIクライアントです。openapi-generator-cliを使用して生成されています。

## 生成方法

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

#### 直接Dockerコマンドで生成（手動swagger.json更新）
```bash
cd src/Pictyping.UnityClient
docker run --rm -v %cd%:/local openapitools/openapi-generator-cli:latest generate \
  -i /local/swagger.json \
  -g csharp \
  -o /local/generated \
  --additional-properties=library=unityWebRequest,packageName=Pictyping.UnityClient,packageVersion=1.0.0,targetFramework=netstandard2.1,packageCompany=Pictyping,sourceFolder=src
```

### swagger.json自動更新について

generate.sh と generate.bat は実行時に自動的に以下を行います：

1. **APIサーバーから最新のswagger.jsonをダウンロード** (`http://localhost:5000/swagger/v1/swagger.json`)
2. **エラーハンドリング**: APIサーバーが停止している場合はエラーメッセージを表示して終了
3. **Unity APIクライアントの生成**: ダウンロード成功後にコード生成を実行

これにより、常に最新のAPI仕様に基づいたUnity Clientが生成されます。

## Unity向けの注意事項

生成されたコードはUnityWebRequestを使用するため、Unity環境でのみ動作します。通常の.NETプロジェクトでのビルドにはUnityEngineへの参照が必要です。

## ディレクトリ構造

```
src/Pictyping.UnityClient/
├── swagger.json                 # OpenAPI仕様ファイル
├── openapi-config.json          # 生成設定
├── generate.sh                  # Linux/Mac用生成スクリプト
├── generate.bat                 # Windows用生成スクリプト
├── Pictyping.UnityClient.csproj # .NETプロジェクトファイル（参考用）
└── generated/                   # 生成されたコード
    └── src/
        ├── Pictyping.UnityClient/
        │   ├── Api/             # APIクライアント
        │   ├── Client/          # HTTPクライアント実装
        │   ├── Model/           # データモデル
        │   └── *.asmdef         # Unity Assembly Definition
        └── Pictyping.UnityClient.Test/
```

## Unityプロジェクトへの統合

1. `generated/src/Pictyping.UnityClient`フォルダをUnityプロジェクトのAssetsフォルダにコピー
2. Newtonsoft.Json for Unityパッケージをインストール
3. 必要に応じてasmdefファイルを編集

## APIクラス

- **AuthApi**: 認証関連のAPI
- **RankingApi**: ランキング関連のAPI
- **PictypingAPIApi**: その他のAPI