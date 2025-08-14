import { useEffect } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAppDispatch } from '../store/hooks'
import { setUser } from '../store/authSlice'
import { AuthService } from '../api/generated'

const AuthCallback = () => {
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const [searchParams] = useSearchParams()

  useEffect(() => {
    const handleCallback = async () => {
      const returnUrl = searchParams.get('returnUrl') || '/'

      try {
        // Cookie認証後、ユーザー情報を取得
        const user = await AuthService.getApiAuthMe()
        dispatch(setUser(user))
        
        // 元のページへリダイレクト
        navigate(returnUrl)
      } catch (error) {
        console.error('Authentication failed:', error)
        navigate('/login')
      }
    }

    handleCallback()
  }, [dispatch, navigate, searchParams])

  return (
    <div style={{ 
      display: 'flex', 
      justifyContent: 'center', 
      alignItems: 'center', 
      height: '100vh' 
    }}>
      <div>
        <h2>ログイン処理中...</h2>
        <p>しばらくお待ちください。</p>
      </div>
    </div>
  )
}

export default AuthCallback