import { Outlet, Link, useNavigate } from 'react-router-dom'
import { useAppSelector, useAppDispatch } from '../store/hooks'
import { logout } from '../store/authSlice'

const Layout = () => {
  const { user, isAuthenticated } = useAppSelector((state) => state.auth)
  const dispatch = useAppDispatch()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await dispatch(logout())
    navigate('/')
  }

  return (
    <div className="app-layout">
      <header className="app-header">
        <div className="header-content">
          <Link to="/" className="logo">
            <img src="/pictyping-logo.png" alt="Pictyping Logo" />
            <h1>Pictyping</h1>
          </Link>
          
          <nav className="main-nav">
            <Link to="/" className="nav-link">ホーム</Link>
            <Link to="/ranking" className="nav-link">ランキング</Link>
            {isAuthenticated && (
              <Link to="/profile" className="nav-link">プロフィール</Link>
            )}
          </nav>

          <div className="user-menu">
            {isAuthenticated ? (
              <>
                <span className="user-name">{user?.displayName || user?.email}</span>
                <button onClick={handleLogout} className="logout-button">
                  ログアウト
                </button>
              </>
            ) : (
              <Link to="/login" className="login-button">
                ログイン
              </Link>
            )}
          </div>
        </div>
      </header>

      <main className="app-main">
        <Outlet />
      </main>

      <footer className="app-footer">
        <div className="footer-content">
          <div className="footer-links">
            <Link to="/privacy">プライバシーポリシー</Link>
            <Link to="/terms">利用規約</Link>
            <Link to="/contact">お問い合わせ</Link>
          </div>
          <p>&copy; 2024 Pictyping. All rights reserved.</p>
        </div>
      </footer>
    </div>
  )
}

export default Layout