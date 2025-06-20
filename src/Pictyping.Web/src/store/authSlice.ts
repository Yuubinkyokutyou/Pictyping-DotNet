import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import { AuthService } from '../api/generated'
import type { LoginRequest } from '../api/generated'

interface User {
  id: number
  email: string
  displayName?: string
  rating: number
  isAdmin: boolean
}

interface AuthState {
  user: User | null
  isAuthenticated: boolean
  loading: boolean
  error: string | null
}

const initialState: AuthState = {
  user: null,
  isAuthenticated: false,
  loading: false,
  error: null,
}

// 非同期アクション
export const checkAuthStatus = createAsyncThunk(
  'auth/checkStatus',
  async () => {
    const user = await AuthService.getApiAuthMe()
    return user
  }
)

export const login = createAsyncThunk(
  'auth/login',
  async ({ email, password }: { email: string; password: string }) => {
    const loginRequest: LoginRequest = { email, password }
    const response = await AuthService.postApiAuthLogin({ body: loginRequest })
    return response
  }
)

export const logout = createAsyncThunk(
  'auth/logout',
  async () => {
    // ローカルストレージからトークンを削除
    localStorage.removeItem('token')
    // Note: サーバーサイドのログアウトエンドポイントが必要な場合は、
    // 生成されたAPIクライアントに追加する必要があります
  }
)

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setUser: (state, action: PayloadAction<User>) => {
      state.user = action.payload
      state.isAuthenticated = true
    },
    clearAuth: (state) => {
      state.user = null
      state.isAuthenticated = false
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      // checkAuthStatus
      .addCase(checkAuthStatus.pending, (state) => {
        state.loading = true
      })
      .addCase(checkAuthStatus.fulfilled, (state, action) => {
        state.loading = false
        state.user = action.payload
        state.isAuthenticated = true
      })
      .addCase(checkAuthStatus.rejected, (state) => {
        state.loading = false
        state.isAuthenticated = false
      })
      // login
      .addCase(login.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false
        // レスポンス構造に応じて調整が必要な場合があります
        state.user = action.payload?.user || action.payload
        state.isAuthenticated = true
        if (action.payload?.token) {
          localStorage.setItem('token', action.payload.token)
        }
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || 'ログインに失敗しました'
      })
      // logout
      .addCase(logout.fulfilled, (state) => {
        state.user = null
        state.isAuthenticated = false
        localStorage.removeItem('token')
      })
  },
})

export const { setUser, clearAuth } = authSlice.actions
export default authSlice.reducer