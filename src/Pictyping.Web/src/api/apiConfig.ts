import { OpenAPI } from './generated/core/OpenAPI'
import { axiosInstance } from '../services/authService'

// OpenAPI設定を更新
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

// 既存のaxiosインスタンスも引き続き使用可能
export { axiosInstance }