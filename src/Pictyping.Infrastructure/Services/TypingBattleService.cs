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
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<OnesideTwoPlayerTypingMatch> CreateMatchAsync(int userId)
    {
        var match = new OnesideTwoPlayerTypingMatch
        {
            UserId = userId,
            BattleStatus = "pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.OnesideTwoPlayerTypingMatches.Add(match);
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task<OnesideTwoPlayerTypingMatch> UpdateMatchAsync(OnesideTwoPlayerTypingMatch match)
    {
        match.UpdatedAt = DateTime.UtcNow;
        _context.Entry(match).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task<IEnumerable<OnesideTwoPlayerTypingMatch>> GetUserMatchesAsync(int userId)
    {
        return await _context.OnesideTwoPlayerTypingMatches
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<OnesideTwoPlayerTypingMatch?> GetActiveMatchAsync(int userId)
    {
        return await _context.OnesideTwoPlayerTypingMatches
            .Where(m => m.UserId == userId && m.BattleStatus == "active")
            .FirstOrDefaultAsync();
    }
}