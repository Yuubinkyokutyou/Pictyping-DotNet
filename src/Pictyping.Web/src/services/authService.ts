import { AuthService } from '../api/generated'
import type { LoginRequest } from '../api/generated'

// OpenAPI設定はapiConfig.tsで行われるため、ここでは行わない
// apiConfig.tsがmain.tsxでインポートされることで初期化される

// 401エラーハンドリング用のラッパー関数
const handleAuthError = async <T>(apiCall: () => Promise<T>): Promise<T> => {
  try {
    return await apiCall()
  } catch (error: any) {
    if (error?.status === 401 || error?.statusCode === 401) {
      // トークンが無効な場合
      localStorage.removeItem('token')
      
      // 開発環境では何もしない（リダイレクトしない）
      if (import.meta.env.DEV) {
        throw error
      }
      
      // 本番環境でのみドメイン間処理
      const currentDomain = window.location.hostname
      const isOldDomain = currentDomain === 'pictyping.com'
      
      if (isOldDomain) {
        // 旧ドメインから新ドメインへリダイレクト
        window.location.href = `https://new.pictyping.com/auth/cross-domain?returnUrl=${encodeURIComponent(window.location.pathname)}`
      }
    }
    throw error
  }
}

const authService = {
  login: async (email: string, password: string) => {
    const loginRequest: LoginRequest = { email, password }
    const response = await handleAuthError(() => 
      AuthService.postApiAuthLogin({ body: loginRequest })
    )
    
    // トークンを保存
    if (response?.token) {
      localStorage.setItem('token', response.token)
    }
    
    return response
  },

  logout: async () => {
    localStorage.removeItem('token')
    // Note: logout endpoint is not available in the API yet
    // TODO: Add logout endpoint to the API and use it here
  },

  getCurrentUser: async () => {
    return await handleAuthError(() => AuthService.getApiAuthMe())
  },

  // ドメイン間認証用
  handleCrossDomainAuth: async (token: string) => {
    localStorage.setItem('token', token)
    return await handleAuthError(() => 
      AuthService.getApiAuthCrossDomainLogin({ token })
    )
  },

  // 旧システムへのリダイレクト用トークン取得
  getRedirectToken: async (targetPath: string) => {
    return await handleAuthError(() => 
      AuthService.getApiAuthRedirectToLegacy({ targetPath })
    )
  },

  // 認証コードをトークンと交換
  exchangeCode: async (code: string) => {
    return await handleAuthError(() => 
      AuthService.postApiAuthExchangeCode({ body: { code } })
    )
  },
}

export default authService