using Microsoft.EntityFrameworkCore;
using Pictyping.Core.Entities;
using Pictyping.Core.Interfaces;
using Pictyping.Infrastructure.Data;

namespace Pictyping.Infrastructure.Services;

public class TypingBattleService : ITypingBattleService
{
    private readonly PictypingDbContext _context;

    public TypingBattleService(PictypingDbContext context)
    {
        _context = context;
    }

    public async Task<OnesideTwoPlayerTypingMatch?> GetMatchByIdAsync(int id)
    {
        return await _context.OnesideTwoPlayerTypingMatches
            .Include(m => m.RegisterUser)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<OnesideTwoPlayerTypingMatch> CreateMatchAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        var match = new OnesideTwoPlayerTypingMatch
        {
            RegisterId = userId,
            MatchId = Guid.NewGuid().ToString(),
            StartedRating = user.Rating,
            EnemyStartedRating = 1200, // Default rating
            BattleStatus = 0, // pending
            IsFinished = false
        };

        _context.OnesideTwoPlayerTypingMatches.Add(match);
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task<OnesideTwoPlayerTypingMatch> UpdateMatchAsync(OnesideTwoPlayerTypingMatch match)
    {
        _context.Entry(match).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task<IEnumerable<OnesideTwoPlayerTypingMatch>> GetUserMatchesAsync(int userId)
    {
        return await _context.OnesideTwoPlayerTypingMatches
            .Where(m => m.RegisterId == userId || m.EnemyId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<OnesideTwoPlayerTypingMatch?> GetActiveMatchAsync(int userId)
    {
        return await _context.OnesideTwoPlayerTypingMatches
            .Where(m => (m.RegisterId == userId || m.EnemyId == userId) && m.BattleStatus == 1)
            .FirstOrDefaultAsync();
    }
}