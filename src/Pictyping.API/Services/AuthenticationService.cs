using Microsoft.EntityFrameworkCore;
using Pictyping.Core.Entities;
using Pictyping.Infrastructure.Data;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

namespace Pictyping.API.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly PictypingDbContext _context;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        PictypingDbContext context,
        IConnectionMultiplexer redis,
        IConfiguration configuration)
    {
        _context = context;
        _redis = redis;
        _configuration = configuration;
    }

    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        // Deviseと同じ方式でパスワードを検証（BCrypt）
        // 注：実際の実装では BCrypt.Net-Next パッケージを使用
        // if (!BCrypt.Net.BCrypt.Verify(password, user.EncryptedPassword))
        //     return null;

        return user;
    }

    public async Task<User> FindOrCreateUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null) return user;

        user = new User
        {
            Email = email,
            EncryptedPassword = "", // OAuthユーザーはパスワードなし
            Guest = false,
            Rating = 1200,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task SaveSessionAsync(string userId, string token)
    {
        var db = _redis.GetDatabase();
        var key = $"session:{userId}";
        await db.StringSetAsync(key, token, TimeSpan.FromHours(24));
    }

    public async Task<string> GenerateTemporaryTokenAsync(string userId)
    {
        // 一時的なトークンを生成（5分間有効）
        var tokenData = $"{userId}:{DateTime.UtcNow.Ticks}:{Guid.NewGuid()}";
        var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
        var token = Convert.ToBase64String(tokenBytes);

        // Redisに保存
        var db = _redis.GetDatabase();
        var key = $"temp_token:{token}";
        await db.StringSetAsync(key, userId, TimeSpan.FromMinutes(5));

        return token;
    }

    // Domain Migration Strategy Implementation
    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}

public class UserService : IUserService
{
    private readonly PictypingDbContext _context;

    public UserService(PictypingDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

public class TypingBattleService : ITypingBattleService
{
    private readonly PictypingDbContext _context;

    public TypingBattleService(PictypingDbContext context)
    {
        _context = context;
    }

    public async Task<OnesideTwoPlayerTypingMatch> StartBattleAsync(int userId, int? enemyUserId)
    {
        var user = await _context.Users.FindAsync(userId);
        var enemyUser = enemyUserId.HasValue ? await _context.Users.FindAsync(enemyUserId.Value) : null;
        
        var match = new OnesideTwoPlayerTypingMatch
        {
            RegisterId = userId,
            EnemyId = enemyUserId,
            MatchId = Guid.NewGuid().ToString(),
            StartedRating = user?.Rating ?? 1200,
            EnemyStartedRating = enemyUser?.Rating ?? 1200,
            BattleStatus = 1, // started
            IsFinished = false
        };

        _context.OnesideTwoPlayerTypingMatches.Add(match);
        await _context.SaveChangesAsync();

        return match;
    }

    public async Task<OnesideTwoPlayerTypingMatch> FinishBattleAsync(
        int matchId, int score, double accuracy, double typeSpeed)
    {
        var match = await _context.OnesideTwoPlayerTypingMatches.FindAsync(matchId);
        if (match == null) throw new InvalidOperationException("Match not found");

        // Rails schema doesn't have Score, Accuracy, TypeSpeed directly
        // These would be stored in BattleDataJson if needed
        match.BattleStatus = 2; // finished
        match.IsFinished = true;

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
}

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
            .Where(u => !u.Guest)
            .OrderByDescending(u => u.Rating)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetUserRankAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return 0;

        var rank = await _context.Users
            .Where(u => !u.Guest && u.Rating > user.Rating)
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