import axios from 'axios'

const API_URL = '/api/auth'

// Axiosインスタンスの作成
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '',
  headers: {
    'Content-Type': 'application/json',
  },
})

// リクエストインターセプター
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
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
      // トークンが無効な場合
      localStorage.removeItem('token')
      
      // 現在のドメインを確認
      const currentDomain = window.location.hostname
      const isOldDomain = currentDomain === 'pictyping.com'
      
      if (isOldDomain) {
        // 旧ドメインから新ドメインへリダイレクト
        window.location.href = `https://new.pictyping.com/auth/cross-domain?returnUrl=${encodeURIComponent(window.location.pathname)}`
      } else {
        // 新ドメインでログインページへ
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  }
)

const authService = {
  login: async (email: string, password: string) => {
    const response = await axiosInstance.post(`${API_URL}/login`, { email, password })
    return response
  },

  logout: async () => {
    localStorage.removeItem('token')
    // 両ドメインのセッションをクリア
    await axiosInstance.post(`${API_URL}/logout`)
  },

  getCurrentUser: async () => {
    return await axiosInstance.get(`${API_URL}/me`)
  },

  // ドメイン間認証用
  handleCrossDomainAuth: async (token: string) => {
    localStorage.setItem('token', token)
    return await axiosInstance.get(`${API_URL}/me`)
  },

  // 旧システムへのリダイレクト用トークン取得
  getRedirectToken: async (targetPath: string) => {
    const response = await axiosInstance.get(`${API_URL}/redirect-to-legacy`, {
      params: { targetPath }
    })
    return response.data
  },
}

export default authService
export { axiosInstance }