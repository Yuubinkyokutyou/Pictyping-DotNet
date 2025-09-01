import { useEffect, useState, useRef } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAppDispatch } from '../store/hooks'
import { setUser } from '../store/authSlice'
import { AuthService } from '../api/generated'

const AuthCallback = () => {
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const [searchParams] = useSearchParams()
  const [error, setError] = useState<string | null>(null)
  const hasExecutedRef = useRef(false)

  useEffect(() => {
    // React.StrictModeによる重複実行を防止
    if (hasExecutedRef.current) {
      return
    }

    const handleCallback = async () => {
      // 実行フラグを立てる
      hasExecutedRef.current = true
      const code = searchParams.get('code')
      const returnUrl = searchParams.get('returnUrl') || '/'

      if (!code) {
        // 旧バージョンとの互換性のため、tokenパラメータもチェック
        const token = searchParams.get('token')
        if (token) {
          console.warn('Using legacy token parameter. This will be deprecated.')
          localStorage.setItem('token', token)
          try {
            const user = await AuthService.getApiAuthMe()
            dispatch(setUser(user))
            navigate(returnUrl)
            return
          } catch (error) {
            console.error('Legacy authentication failed:', error)
            navigate('/login')
            return
          }
        }
        
        setError('認証コードが見つかりません')
        setTimeout(() => navigate('/login'), 1000)
        return
      }

      try {
        // 認証コードをトークンと交換
        const data = await AuthService.postApiAuthExchangeCode({
          body: { code }
        })
        
        if (!data || !data.token) {
          throw new Error('Code exchange failed - no token received')
        }
        
        // トークンを保存
        localStorage.setItem('token', data.token)
        
        // ユーザー情報をストアに保存
        if (data.user) {
          dispatch(setUser(data.user))
        } else {
          // ユーザー情報が含まれていない場合は別途取得
          const user = await AuthService.getApiAuthMe()
          dispatch(setUser(user))
        }
        
        // 元のページへリダイレクト
        navigate(returnUrl)
      } catch (error) {
        console.error('Authentication failed:', error)
        setError('認証に失敗しました')
        setTimeout(() => navigate('/login'), 1000)
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
        {error ? (
          <>
            <h2 style={{ color: 'red' }}>エラー</h2>
            <p>{error}</p>
            <p>ログインページにリダイレクトしています...</p>
          </>
        ) : (
          <>
            <h2>ログイン処理中...</h2>
            <p>しばらくお待ちください。</p>
          </>
        )}
      </div>
    </div>
  )
}

export default AuthCallback