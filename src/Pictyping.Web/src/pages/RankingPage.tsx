import { useEffect, useState } from 'react'
import { RankingService } from '../api/generated'

interface RankingUser {
  id: number
  displayName?: string
  email: string
  rating: number
}

const RankingPage = () => {
  const [rankings, setRankings] = useState<RankingUser[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchRankings = async () => {
      try {
        setLoading(true)
        setError(null)
        const response = await RankingService.getApiRanking({ count: 100 })
        setRankings(response || [])
      } catch (err) {
        console.error('Failed to fetch rankings:', err)
        setError('ランキングの取得に失敗しました')
      } finally {
        setLoading(false)
      }
    }

    fetchRankings()
  }, [])

  if (loading) {
    return <div>読み込み中...</div>
  }

  if (error) {
    return <div className="error">{error}</div>
  }

  return (
    <div className="ranking-page">
      <h1>ランキング</h1>
      
      <div className="ranking-table">
        <table>
          <thead>
            <tr>
              <th>順位</th>
              <th>プレイヤー名</th>
              <th>レーティング</th>
            </tr>
          </thead>
          <tbody>
            {rankings.map((user, index) => (
              <tr key={user.id}>
                <td>{index + 1}</td>
                <td>{user.displayName || user.email}</td>
                <td>{user.rating}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default RankingPage