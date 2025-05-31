# Rails側の統合設定

新システムとの認証連携のため、既存のRailsアプリケーションに以下の変更を加える必要があります。

## 1. JWT gem の追加

`Gemfile` に追加:
```ruby
gem 'jwt'
```

## 2. 認証コントローラーの追加

`app/controllers/auth_controller.rb`:
```ruby
class AuthController < ApplicationController
  skip_before_action :verify_authenticity_token, only: [:verify]
  
  # 新システムからのリダイレクトを処理
  def verify
    token = params[:token]
    redirect_path = params[:redirect] || root_path
    
    begin
      # 新システムからの一時トークンを検証
      decoded = JWT.decode(token, Rails.application.secrets.jwt_secret, true, algorithm: 'HS256')
      user_id = decoded[0]['user_id']
      
      # ユーザーをログイン状態にする
      user = User.find(user_id)
      sign_in(user)
      
      redirect_to redirect_path
    rescue JWT::DecodeError => e
      redirect_to new_user_session_path, alert: '認証に失敗しました'
    end
  end
  
  # 新システムへのリダイレクト用トークン生成
  def redirect_to_new_system
    if current_user
      # JWTトークンを生成
      payload = {
        user_id: current_user.id,
        email: current_user.email,
        exp: 5.minutes.from_now.to_i
      }
      
      token = JWT.encode(payload, Rails.application.secrets.jwt_secret, 'HS256')
      
      # 新システムへリダイレクト
      redirect_to "#{ENV['NEW_SYSTEM_URL']}/api/auth/cross-domain-login?token=#{token}&returnUrl=#{params[:return_url]}"
    else
      redirect_to new_user_session_path
    end
  end
end
```

## 3. ルーティングの追加

`config/routes.rb`:
```ruby
# 新システムとの認証連携
get '/auth/verify', to: 'auth#verify'
get '/auth/redirect_to_new', to: 'auth#redirect_to_new_system'
```

## 4. 環境変数の設定

`.env` に追加:
```
NEW_SYSTEM_URL=https://new.pictyping.com
JWT_SECRET=your_shared_jwt_secret_key
```

## 5. CORS設定の更新

`config/initializers/cors.rb`:
```ruby
Rails.application.config.middleware.insert_before 0, Rack::Cors do
  allow do
    origins 'https://new.pictyping.com', 'http://localhost:3000'
    
    resource '*',
      headers: :any,
      methods: [:get, :post, :put, :patch, :delete, :options, :head],
      credentials: true
  end
end
```

## 6. セッション共有の設定

`config/initializers/session_store.rb`:
```ruby
Rails.application.config.session_store :redis_store,
  servers: ["redis://#{ENV['REDIS_HOST']}:#{ENV['REDIS_PORT']}/0/session"],
  expire_after: 90.minutes,
  key: "_pictyping_session",
  domain: :all  # サブドメイン間でセッションを共有
```

## 移行時の注意事項

1. **JWT Secret**: 両システムで同じシークレットキーを使用する必要があります
2. **Redis**: 両システムが同じRedisインスタンスを使用してセッション情報を共有します
3. **CORS**: 新旧ドメイン間の通信を許可する設定が必要です
4. **SSL**: 本番環境では必ずHTTPSを使用してください

## テスト方法

1. 旧システムでログイン
2. 新システムのURLにアクセス
3. 自動的にログイン状態が引き継がれることを確認
4. 逆方向（新→旧）も同様にテスト