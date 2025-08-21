import { Routes, Route, Navigate } from 'react-router-dom'
import { useEffect } from 'react'
import { useAppDispatch } from './store/hooks'
import { checkAuthStatus } from './store/authSlice'
import Layout from './components/Layout'
import HomePage from './pages/HomePage'
import RankingPage from './pages/RankingPage'
import ProfilePage from './pages/ProfilePage'
import PrivacyPage from './pages/PrivacyPage'
import TermsPage from './pages/TermsPage'
import ContactPage from './pages/ContactPage'
import AuthCallback from './pages/AuthCallback'
import CrossDomainAuth from './pages/CrossDomainAuth'
import LoginPage from './pages/LoginPage'

function App() {
  const dispatch = useAppDispatch()

  useEffect(() => {
    // 初回読み込み時に認証状態をチェック
    dispatch(checkAuthStatus())
  }, [dispatch])

  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="ranking" element={<RankingPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="privacy" element={<PrivacyPage />} />
        <Route path="terms" element={<TermsPage />} />
        <Route path="contact" element={<ContactPage />} />
      </Route>
      
      {/* 認証関連のルート */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/auth/callback" element={<AuthCallback />} />
      <Route path="/auth/cross-domain" element={<CrossDomainAuth />} />
      
      {/* 404 */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App