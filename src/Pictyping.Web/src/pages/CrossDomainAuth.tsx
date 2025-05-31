import { useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAppDispatch } from '../store/hooks'
import { setUser } from '../store/authSlice'
import authService from '../services/authService'

/**
 * ドメイン間認証処理コンポーネント
 * 旧システムから新システムへリダイレクトされた際の認証処理を行う
 */
const CrossDomainAuth = () => {
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const [searchParams] = useSearchParams()

  useEffect(() => {
    const handleAuth = async () => {
      const token = searchParams.get('token')
      const returnUrl = searchParams.get('returnUrl') || '/'

      if (!token) {
        console.error('No token provided')
        navigate('/login')
        return
      }

      try {
        // トークンを使って認証
        const response = await authService.handleCrossDomainAuth(token)
        dispatch(setUser(response.data))
        
        // 元のページへリダイレクト
        navigate(returnUrl)
      } catch (error) {
        console.error('Cross domain authentication failed:', error)
        navigate('/login')
      }
    }

    handleAuth()
  }, [dispatch, navigate, searchParams])

  return (
    <div style={{ 
      display: 'flex', 
      justifyContent: 'center', 
      alignItems: 'center', 
      height: '100vh' 
    }}>
      <div>
        <h2>認証処理中...</h2>
        <p>しばらくお待ちください。</p>
      </div>
    </div>
  )
}

export default CrossDomainAuth