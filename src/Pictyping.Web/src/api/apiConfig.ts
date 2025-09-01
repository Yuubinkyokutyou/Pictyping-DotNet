import { OpenAPI } from './generated/core/OpenAPI'

// OpenAPIクライアントの設定
OpenAPI.BASE = import.meta.env.VITE_API_URL || ''
OpenAPI.WITH_CREDENTIALS = true
OpenAPI.TOKEN = async () => {
  const token = localStorage.getItem('token')
  return token || ''
}

// 生成されたAPIサービスはすべて静的メソッドなので、
// 直接インポートして使用します
export * from './generated/services/AuthService'
export * from './generated/services/RankingService'
export * from './generated/services/PictypingApiService'