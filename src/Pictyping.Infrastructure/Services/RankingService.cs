using Microsoft.EntityFrameworkCore;
using Pictyping.Core.Entities;
using Pictyping.Core.Interfaces;
using Pictyping.Infrastructure.Data;

namespace Pictyping.Infrastructure.Services;

public class RankingService : IRankingService
{
    private readonly PictypingDbContext _context;

    public RankingService(PictypingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetTopRankingsAsync(int count = 100)
    {
        return await _context.Users
            .Where(u => u.Rating > 0)
            .OrderByDescending(u => u.Rating)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetUserRankAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return 0;

        var rank = await _context.Users
            .Where(u => u.Rating > user.Rating)
            .CountAsync();

        return rank + 1;
    }

    public async Task UpdateUserRatingAsync(int userId, int newRating)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Rating = newRating;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}