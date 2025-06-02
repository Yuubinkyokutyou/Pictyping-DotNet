import { AuthService } from '../api/generated'
import type { LoginRequest } from '../api/generated'

/**
 * 自動生成されたAPIクライアントを使用した認証サービス
 * 既存のauthService.tsと併用可能
 */
export const authServiceGenerated = {
  /**
   * ユーザーログイン
   */
  login: async (email: string, password: string) => {
    const loginRequest: LoginRequest = { email, password }
    const response = await AuthService.postApiAuthLogin({ body: loginRequest })
    
    // トークンをローカルストレージに保存
    if (response?.token) {
      localStorage.setItem('token', response.token)
    }
    
    return response
  },

  /**
   * 現在のユーザー情報を取得
   */
  getCurrentUser: async () => {
    return await AuthService.getApiAuthMe()
  },

  /**
   * クロスドメインログイン
   */
  crossDomainLogin: async (token: string, returnUrl?: string) => {
    return await AuthService.getApiAuthCrossDomainLogin({ token, returnUrl })
  },

  /**
   * レガシーシステムへのリダイレクト
   */
  redirectToLegacy: async (targetPath?: string) => {
    return await AuthService.getApiAuthRedirectToLegacy({ targetPath })
  },

  /**
   * Google OAuth ログイン
   */
  googleLogin: () => {
    // Google OAuthはリダイレクトベースなので、直接URLにアクセス
    window.location.href = `${import.meta.env.VITE_API_URL || ''}/api/auth/google`
  },

  /**
   * Google OAuth コールバック処理
   */
  handleGoogleCallback: async () => {
    return await AuthService.getApiAuthGoogleCallback()
  },
}

export default authServiceGenerated