using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pictyping.Core.Entities;
using Pictyping.Core.DTOs;
using Pictyping.Infrastructure.Data;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pictyping.API.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly PictypingDbContext _context;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        PictypingDbContext context,
        IConnectionMultiplexer redis,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _redis = redis;
        _configuration = configuration;
        _logger = logger;
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

    // Migration Token Validation Implementation
    public async Task<MigrationUserInfo?> ValidateMigrationToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var sharedSecret = _configuration["SharedJwtSecret"];
        
        if (string.IsNullOrEmpty(sharedSecret))
        {
            _logger.LogError("SharedJwtSecret not configured");
            return null;
        }

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(sharedSecret)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            
            if (jwtToken == null)
            {
                _logger.LogWarning("Invalid token format");
                return null;
            }
            
            // ワンタイムトークンチェック（オプション）
            var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == "jti")?.Value;
            if (!string.IsNullOrEmpty(jti) && await IsTokenUsed(jti))
            {
                _logger.LogWarning("Token already used: {Jti}", jti);
                throw new SecurityTokenException("Token already used");
            }
            
            // トークンから情報抽出
            var userInfo = new MigrationUserInfo
            {
                UserId = int.Parse(jwtToken.Claims.First(x => x.Type == "user_id").Value),
                Email = jwtToken.Claims.First(x => x.Type == "email").Value,
                Name = jwtToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value,
                Provider = jwtToken.Claims.FirstOrDefault(x => x.Type == "provider")?.Value,
                Admin = bool.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == "admin")?.Value ?? "false"),
                Rating = int.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == "rating")?.Value ?? "1000"),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(
                    long.Parse(jwtToken.Claims.FirstOrDefault(x => x.Type == "created_at")?.Value ?? "0")
                ).DateTime,
                Jti = jti
            };

            // トークンを使用済みとしてマーク
            if (!string.IsNullOrEmpty(jti))
            {
                await MarkTokenAsUsed(jti);
            }

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return null;
        }
    }

    public async Task<User> CreateOrUpdateUserFromMigration(MigrationUserInfo userInfo)
    {
        // 既存ユーザーを検索（EmailまたはRailsのuser_idで）
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userInfo.Email);

        if (existingUser != null)
        {
            // 既存ユーザーの情報を更新
            existingUser.Name = userInfo.Name ?? existingUser.Name;
            existingUser.Provider = userInfo.Provider ?? existingUser.Provider;
            existingUser.Admin = userInfo.Admin;
            existingUser.Rating = userInfo.Rating;
            existingUser.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingUser;
        }
        else
        {
            // 新規ユーザーを作成
            var newUser = new User
            {
                Email = userInfo.Email,
                Name = userInfo.Name,
                Provider = userInfo.Provider,
                Admin = userInfo.Admin,
                Rating = userInfo.Rating,
                EncryptedPassword = "", // OAuth/Migration users don't have passwords
                Guest = false,
                CreatedAt = userInfo.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new InvalidOperationException("JWT secret not configured");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name ?? user.Email),
            new Claim("admin", user.Admin.ToString().ToLower()),
            new Claim("rating", user.Rating.ToString()),
            new Claim("provider", user.Provider ?? "email"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // 7 days expiry
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> IsTokenUsed(string jti)
    {
        if (string.IsNullOrEmpty(jti)) return false;
        
        var db = _redis.GetDatabase();
        var key = $"used_token:{jti}";
        return await db.KeyExistsAsync(key);
    }

    private async Task MarkTokenAsUsed(string jti)
    {
        if (string.IsNullOrEmpty(jti)) return;
        
        var db = _redis.GetDatabase();
        var key = $"used_token:{jti}";
        await db.StringSetAsync(key, "used", TimeSpan.FromHours(24)); // Keep for 24 hours
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