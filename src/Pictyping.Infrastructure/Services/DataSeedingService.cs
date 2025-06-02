using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pictyping.Core.Entities;
using Pictyping.Core.Interfaces;
using Pictyping.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace Pictyping.Infrastructure.Services;

public class DataSeedingService : IDataSeedingService
{
    private readonly PictypingDbContext _context;
    private readonly ILogger<DataSeedingService> _logger;

    public DataSeedingService(PictypingDbContext context, ILogger<DataSeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDevelopmentDataAsync()
    {
        _logger.LogInformation("Starting development data seeding...");

        try
        {
            // Check if data already exists
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Data already exists. Skipping seeding.");
                return;
            }

            // Create test users
            var users = new List<User>
            {
                new User
                {
                    Email = "admin@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    Name = "Admin User",
                    Admin = true,
                    Rating = 1500,
                    Guest = false,
                    PlayfabId = "ADMIN123"
                },
                new User
                {
                    Email = "player1@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    Name = "Player One",
                    Admin = false,
                    Rating = 1200,
                    Guest = false,
                    PlayfabId = "PLAYER001"
                },
                new User
                {
                    Email = "player2@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    Name = "Player Two",
                    Admin = false,
                    Rating = 1350,
                    Guest = false,
                    PlayfabId = "PLAYER002"
                },
                new User
                {
                    Email = "guest@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    Name = "Guest Player",
                    Admin = false,
                    Rating = 1000,
                    Guest = true,
                    PlayfabId = "GUEST001"
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created {users.Count} test users");

            // Create test typing matches
            var matches = new List<OnesideTwoPlayerTypingMatch>
            {
                new OnesideTwoPlayerTypingMatch
                {
                    RegisterId = users[1].Id,
                    EnemyId = users[2].Id,
                    MatchId = "MATCH_DEV_001",
                    StartedRating = users[1].Rating,
                    EnemyStartedRating = users[2].Rating,
                    IsFinished = true,
                    FinishedRating = users[1].Rating + 25,
                    BattleStatus = 2
                },
                new OnesideTwoPlayerTypingMatch
                {
                    RegisterId = users[2].Id,
                    EnemyId = users[1].Id,
                    MatchId = "MATCH_DEV_002",
                    StartedRating = users[2].Rating,
                    EnemyStartedRating = users[1].Rating,
                    IsFinished = true,
                    FinishedRating = users[2].Rating + 30,
                    BattleStatus = 2
                },
                new OnesideTwoPlayerTypingMatch
                {
                    RegisterId = users[1].Id,
                    EnemyId = users[3].Id,
                    MatchId = "MATCH_DEV_003",
                    StartedRating = users[1].Rating,
                    EnemyStartedRating = users[3].Rating,
                    IsFinished = false,
                    BattleStatus = 1
                }
            };

            await _context.OnesideTwoPlayerTypingMatches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created {matches.Count} test typing matches");

            // Create OAuth identities for some users
            var oauthIdentities = new List<OmniAuthIdentity>
            {
                new OmniAuthIdentity
                {
                    UserId = users[0].Id,
                    Provider = "google_oauth2",
                    Uid = "google-admin-123456"
                },
                new OmniAuthIdentity
                {
                    UserId = users[1].Id,
                    Provider = "google_oauth2",
                    Uid = "google-player1-789012"
                }
            };

            await _context.OmniAuthIdentities.AddRangeAsync(oauthIdentities);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Created {oauthIdentities.Count} test OAuth identities");

            _logger.LogInformation("Development data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding");
            throw;
        }
    }

    private string HashPassword(string password)
    {
        // Simple hash for development purposes
        // In production, use proper password hashing like BCrypt
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}