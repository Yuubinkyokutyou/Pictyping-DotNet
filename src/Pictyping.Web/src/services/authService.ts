import axios from 'axios'

const API_URL = '/api/auth'

// Axiosインスタンスの作成
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Cookie送信を有効化
})

// リクエストインターセプター
axiosInstance.interceptors.request.use(
  (config) => {
    // Cookie認証を使用するため、Authorization headerは不要
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
      // 認証エラーの場合、ログインページへリダイレクト
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
    return response
  },

  logout: async () => {
    // Cookie認証のログアウト
    await axiosInstance.post(`${API_URL}/logout`)
  },

  getCurrentUser: async () => {
    return await axiosInstance.get(`${API_URL}/me`)
  },

}

export default authService
export { axiosInstance }