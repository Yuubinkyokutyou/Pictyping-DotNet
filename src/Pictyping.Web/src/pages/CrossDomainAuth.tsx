import { useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAppDispatch } from '../store/hooks'
import { setAuthToken } from '../store/authSlice'
import { AuthService } from '../api/generated'

/**
 * リダイレクト処理コンポーネント (Domain Migration Strategy Implementation)
 * 旧システムから新システムへリダイレクトされた際の認証処理を行う
 */
const CrossDomainAuth = () => {
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const [searchParams] = useSearchParams()

  useEffect(() => {
    const handleAuth = async () => {
      const token = searchParams.get('token')

      if (token) {
        try {
          // JWTトークンをlocalStorageに保存
          localStorage.setItem('authToken', token)
          
          // Reduxストアを更新
          dispatch(setAuthToken(token))
          
          // ユーザー情報を取得して確認
          const user = await AuthService.getApiAuthMe()
          console.log('Migration successful for user:', user)
          
          // ホームページにリダイレクト
          navigate('/')
        } catch (error) {
          console.error('Migration token validation failed:', error)
          navigate('/login?error=migration_failed')
        }
      } else {
        console.error('No migration token provided')
        navigate('/login?error=migration_failed')
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
        <h2>認証情報を移行中...</h2>
        <p>しばらくお待ちください。</p>
      </div>
    </div>
  )
}

export default CrossDomainAuth