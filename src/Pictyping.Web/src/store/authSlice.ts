import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import authService from '../services/authService'

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
    const response = await authService.getCurrentUser()
    return response.data
  }
)

// Password login has been removed - only OAuth authentication is supported

export const logout = createAsyncThunk(
  'auth/logout',
  async () => {
    await authService.logout()
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
      // Password login has been removed
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