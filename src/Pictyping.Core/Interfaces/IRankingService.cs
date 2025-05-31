using Pictyping.Core.Entities;

namespace Pictyping.Core.Interfaces;

public interface IRankingService
{
    Task<IEnumerable<User>> GetTopRankingsAsync(int count = 100);
    Task<int> GetUserRankAsync(int userId);
    Task UpdateUserRatingAsync(int userId, int newRating);
}