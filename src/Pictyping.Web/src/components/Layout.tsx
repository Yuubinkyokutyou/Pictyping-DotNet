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
    <div className="min-h-screen flex flex-col bg-white">
      <header className="bg-white text-gray-900 py-6 border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-8 flex justify-between items-center gap-8 flex-wrap md:flex-nowrap">
          <Link to="/" className="flex items-center no-underline gap-3">
            <img src="/pictyping-logo.png" alt="Pictyping Logo" className="h-8 w-auto" />
            <h1 className="text-2xl font-semibold text-blue-600 tracking-tight">Pictyping</h1>
          </Link>
          
          <nav className="flex gap-12 flex-1 justify-center">
            <Link to="/" className="text-gray-500 no-underline font-medium text-sm transition-colors hover:text-blue-600 relative after:content-[''] after:absolute after:bottom-[-4px] after:left-0 after:w-0 after:h-0.5 after:bg-blue-600 after:transition-all hover:after:w-full">ホーム</Link>
            <Link to="/ranking" className="text-gray-500 no-underline font-medium text-sm transition-colors hover:text-blue-600 relative after:content-[''] after:absolute after:bottom-[-4px] after:left-0 after:w-0 after:h-0.5 after:bg-blue-600 after:transition-all hover:after:w-full">ランキング</Link>
            {isAuthenticated && (
              <Link to="/profile" className="text-gray-500 no-underline font-medium text-sm transition-colors hover:text-blue-600 relative after:content-[''] after:absolute after:bottom-[-4px] after:left-0 after:w-0 after:h-0.5 after:bg-blue-600 after:transition-all hover:after:w-full">プロフィール</Link>
            )}
          </nav>

          <div className="flex items-center gap-4">
            {isAuthenticated ? (
              <>
                <span className="text-gray-500 text-sm">{user?.displayName || user?.email}</span>
                <button onClick={handleLogout} className="bg-blue-600 text-white border-0 px-6 py-2 rounded-full cursor-pointer font-medium text-sm transition-all hover:bg-blue-700 hover:-translate-y-px">
                  ログアウト
                </button>
              </>
            ) : (
              <Link to="/login" className="bg-blue-600 text-white border-0 px-6 py-2 rounded-full cursor-pointer no-underline inline-block font-medium text-sm transition-all hover:bg-blue-700 hover:-translate-y-px">
                ログイン
              </Link>
            )}
          </div>
        </div>
      </header>

      <main className="flex-1 max-w-7xl mx-auto px-8 py-12 w-full">
        <Outlet />
      </main>

      <footer className="bg-gray-50 text-gray-500 py-12 mt-auto border-t border-gray-200">
        <div className="max-w-7xl mx-auto px-8 text-center">
          <div className="flex justify-center gap-12 mb-6">
            <Link to="/privacy" className="text-gray-500 no-underline text-sm transition-colors hover:text-blue-600">プライバシーポリシー</Link>
            <Link to="/terms" className="text-gray-500 no-underline text-sm transition-colors hover:text-blue-600">利用規約</Link>
            <Link to="/contact" className="text-gray-500 no-underline text-sm transition-colors hover:text-blue-600">お問い合わせ</Link>
          </div>
          <p>&copy; 2024 Pictyping. All rights reserved.</p>
        </div>
      </footer>
    </div>
  )
}

export default Layout