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
    return <div className="text-center py-8 text-gray-500">読み込み中...</div>
  }

  if (error) {
    return <div className="text-center py-8 text-red-500">{error}</div>
  }

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-4xl text-blue-700 text-center mb-12 font-semibold">ランキング</h1>
      
      <div className="w-full bg-white rounded-2xl overflow-hidden shadow-sm">
        <table className="w-full">
          <thead>
            <tr className="bg-gray-50">
              <th className="px-4 py-4 text-left border-b border-gray-200 font-semibold text-gray-700 text-xs uppercase tracking-wider">順位</th>
              <th className="px-4 py-4 text-left border-b border-gray-200 font-semibold text-gray-700 text-xs uppercase tracking-wider">プレイヤー名</th>
              <th className="px-4 py-4 text-left border-b border-gray-200 font-semibold text-gray-700 text-xs uppercase tracking-wider">レーティング</th>
            </tr>
          </thead>
          <tbody>
            {rankings.map((user, index) => (
              <tr key={user.id} className="hover:bg-gray-50 transition-colors">
                <td className="px-4 py-4 text-left border-b border-gray-200 last:border-b-0">{index + 1}</td>
                <td className="px-4 py-4 text-left border-b border-gray-200 last:border-b-0">{user.displayName || user.email}</td>
                <td className="px-4 py-4 text-left border-b border-gray-200 last:border-b-0">{user.rating}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default RankingPage