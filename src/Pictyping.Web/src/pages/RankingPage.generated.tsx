import { useEffect, useState } from 'react'
import { RankingService } from '../api/generated'

interface RankingUser {
  id: number
  displayName?: string
  email: string
  rating: number
}

/**
 * 自動生成されたAPIクライアントを使用したランキングページの実装例
 */
const RankingPageGenerated = () => {
  const [rankings, setRankings] = useState<RankingUser[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [count, setCount] = useState(100)

  useEffect(() => {
    const fetchRankings = async () => {
      try {
        setLoading(true)
        const response = await RankingService.getApiRanking({ count })
        setRankings(response || [])
      } catch (err) {
        console.error('Failed to fetch rankings:', err)
        setError('ランキングの取得に失敗しました')
      } finally {
        setLoading(false)
      }
    }

    fetchRankings()
  }, [count])

  const fetchUserRanking = async (userId: number) => {
    try {
      const userRanking = await RankingService.getApiRankingUser({ userId })
      console.log('User ranking:', userRanking)
    } catch (err) {
      console.error('Failed to fetch user ranking:', err)
    }
  }

  if (loading) {
    return <div className="loading">読み込み中...</div>
  }

  if (error) {
    return <div className="error">{error}</div>
  }

  return (
    <div className="ranking-page">
      <h1>ランキング</h1>
      
      <div className="ranking-controls">
        <label>
          表示件数:
          <select value={count} onChange={(e) => setCount(Number(e.target.value))}>
            <option value={50}>50</option>
            <option value={100}>100</option>
            <option value={200}>200</option>
          </select>
        </label>
      </div>

      <div className="ranking-table">
        <table>
          <thead>
            <tr>
              <th>順位</th>
              <th>プレイヤー名</th>
              <th>レーティング</th>
              <th>詳細</th>
            </tr>
          </thead>
          <tbody>
            {rankings.map((user, index) => (
              <tr key={user.id}>
                <td>{index + 1}</td>
                <td>{user.displayName || user.email}</td>
                <td>{user.rating}</td>
                <td>
                  <button onClick={() => fetchUserRanking(user.id)}>
                    詳細を見る
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default RankingPageGenerated