import { Link } from 'react-router-dom'
import { useAppSelector } from '../store/hooks'

const HomePage = () => {
  const { user, isAuthenticated } = useAppSelector((state) => state.auth)

  return (
    <div>
      <div className="text-center py-20 px-8 bg-gradient-to-b from-blue-50 to-white -mx-8 -mt-12 mb-12">
        <h1 className="text-5xl mb-6 text-blue-700 font-bold tracking-tight">Pictyping へようこそ</h1>
        <p className="text-lg text-gray-500 mb-8 max-w-2xl mx-auto">画像を見ながら楽しくタイピング練習ができるゲームです</p>
        
        {isAuthenticated ? (
          <div>
            <p className="text-gray-600 mb-2">おかえりなさい、{user?.displayName || user?.email}さん！</p>
            <p className="text-gray-600 mb-6">現在のレーティング: {user?.rating}</p>
            <button className="bg-blue-600 text-white border-0 px-12 py-4 text-lg font-medium rounded-full cursor-pointer transition-all shadow-md hover:bg-blue-700 hover:-translate-y-px hover:shadow-lg">ゲームを始める</button>
          </div>
        ) : (
          <div>
            <p className="text-gray-600 mb-6">ゲストとしてプレイすることもできます</p>
            <button className="bg-blue-600 text-white border-0 px-12 py-4 text-lg font-medium rounded-full cursor-pointer transition-all shadow-md hover:bg-blue-700 hover:-translate-y-px hover:shadow-lg">ゲストでプレイ</button>
            <Link to="/login" className="block mt-4 text-blue-600 hover:text-blue-700 underline">ログイン</Link>
          </div>
        )}
      </div>

      <div className="bg-white p-0">
        <h2 className="text-3xl text-blue-700 text-center mb-12 font-semibold">特徴</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-12 mt-16">
          <div className="text-center p-8 bg-gray-50 rounded-2xl transition-transform hover:-translate-y-0.5">
            <h3 className="text-xl mb-3 text-blue-700 font-semibold">🎮 オンライン対戦</h3>
            <p className="text-gray-500 leading-relaxed">他のプレイヤーとリアルタイムで対戦</p>
          </div>
          <div className="text-center p-8 bg-gray-50 rounded-2xl transition-transform hover:-translate-y-0.5">
            <h3 className="text-xl mb-3 text-blue-700 font-semibold">📊 ランキングシステム</h3>
            <p className="text-gray-500 leading-relaxed">レーティングを上げて上位を目指そう</p>
          </div>
          <div className="text-center p-8 bg-gray-50 rounded-2xl transition-transform hover:-translate-y-0.5">
            <h3 className="text-xl mb-3 text-blue-700 font-semibold">🖼️ 画像タイピング</h3>
            <p className="text-gray-500 leading-relaxed">画像を見ながら楽しくタイピング練習</p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default HomePage