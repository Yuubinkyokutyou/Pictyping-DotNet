import { Link } from 'react-router-dom'
import { useAppSelector } from '../store/hooks'

const HomePage = () => {
  const { user, isAuthenticated } = useAppSelector((state) => state.auth)

  return (
    <div className="home-page">
      <div className="hero-section">
        <h1>Pictyping へようこそ</h1>
        <p>画像を見ながら楽しくタイピング練習ができるゲームです</p>
        
        {isAuthenticated ? (
          <div className="user-welcome">
            <p>おかえりなさい、{user?.displayName || user?.email}さん！</p>
            <p>現在のレーティング: {user?.rating}</p>
            <button className="play-button">ゲームを始める</button>
          </div>
        ) : (
          <div className="guest-welcome">
            <p>ゲストとしてプレイすることもできます</p>
            <button className="play-button">ゲストでプレイ</button>
            <Link to="/login" className="login-link">ログイン</Link>
          </div>
        )}
      </div>

      <div className="features">
        <h2>特徴</h2>
        <div className="feature-list">
          <div className="feature-item">
            <h3>🎮 オンライン対戦</h3>
            <p>他のプレイヤーとリアルタイムで対戦</p>
          </div>
          <div className="feature-item">
            <h3>📊 ランキングシステム</h3>
            <p>レーティングを上げて上位を目指そう</p>
          </div>
          <div className="feature-item">
            <h3>🖼️ 画像タイピング</h3>
            <p>画像を見ながら楽しくタイピング練習</p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default HomePage