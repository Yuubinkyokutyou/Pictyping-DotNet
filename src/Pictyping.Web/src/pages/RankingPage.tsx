import { useEffect, useState } from 'react'
import { axiosInstance } from '../services/authService'

interface RankingUser {
  id: number
  displayName?: string
  email: string
  rating: number
}

const RankingPage = () => {
  const [rankings, setRankings] = useState<RankingUser[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchRankings = async () => {
      try {
        const response = await axiosInstance.get('/api/rankings')
        setRankings(response.data)
      } catch (error) {
        console.error('Failed to fetch rankings:', error)
      } finally {
        setLoading(false)
      }
    }

    fetchRankings()
  }, [])

  if (loading) {
    return <div>読み込み中...</div>
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