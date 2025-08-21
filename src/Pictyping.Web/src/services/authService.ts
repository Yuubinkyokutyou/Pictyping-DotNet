import axios from 'axios'

const API_URL = '/api/auth'
const TOKEN_KEY = 'auth_token'

// トークン管理
const tokenManager = {
  getToken: () => localStorage.getItem(TOKEN_KEY),
  setToken: (token: string) => localStorage.setItem(TOKEN_KEY, token),
  removeToken: () => localStorage.removeItem(TOKEN_KEY),
}

// Axiosインスタンスの作成
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: false, // JWTトークン認証なのでCookie送信は不要
})

// リクエストインターセプター
axiosInstance.interceptors.request.use(
  (config) => {
    // トークンがあればAuthorizationヘッダーに追加
    const token = tokenManager.getToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// レスポンスインターセプター
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // 認証エラーの場合、トークンを削除してログインページへリダイレクト
      tokenManager.removeToken()
      if (!window.location.pathname.includes('/login')) {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

const authService = {
  login: async (email: string, password: string) => {
    const response = await axiosInstance.post(`${API_URL}/login`, { email, password })
    // トークンを保存
    if (response.data?.token) {
      tokenManager.setToken(response.data.token)
    }
    return response
  },

  logout: async () => {
    // サーバーにログアウトリクエストを送信
    await axiosInstance.post(`${API_URL}/logout`)
    // ローカルのトークンを削除
    tokenManager.removeToken()
  },

  getCurrentUser: async () => {
    return await axiosInstance.get(`${API_URL}/me`)
  },

  // トークン管理用のメソッドをエクスポート
  setToken: tokenManager.setToken,
  getToken: tokenManager.getToken,
  removeToken: tokenManager.removeToken,
}

export default authService
export { axiosInstance, tokenManager }