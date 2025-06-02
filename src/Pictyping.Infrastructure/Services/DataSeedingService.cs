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
                    DisplayName = "Admin User",
                    Admin = true,
                    Rating = 1500,
                    Guest = false,
                    PlayFabId = "ADMIN123"
                },
                new User
                {
                    Email = "player1@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    DisplayName = "Player One",
                    Admin = false,
                    Rating = 1200,
                    Guest = false,
                    PlayFabId = "PLAYER001"
                },
                new User
                {
                    Email = "player2@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    DisplayName = "Player Two",
                    Admin = false,
                    Rating = 1350,
                    Guest = false,
                    PlayFabId = "PLAYER002"
                },
                new User
                {
                    Email = "guest@example.com",
                    EncryptedPassword = HashPassword("password123"),
                    DisplayName = "Guest Player",
                    Admin = false,
                    Rating = 1000,
                    Guest = true,
                    PlayFabId = "GUEST001"
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
                    UserId = users[1].Id,
                    EnemyUserId = users[2].Id,
                    Score = 850,
                    Accuracy = 95.5,
                    TypeSpeed = 4.2,
                    MissCount = 5,
                    BattleTime = 60,
                    QuestionContents = "The quick brown fox jumps over the lazy dog",
                    InputContents = "The quick brown fox jumps over the lazy dog",
                    MissTypeContents = "[]",
                    BattleStatus = "completed"
                },
                new OnesideTwoPlayerTypingMatch
                {
                    UserId = users[2].Id,
                    EnemyUserId = users[1].Id,
                    Score = 920,
                    Accuracy = 98.2,
                    TypeSpeed = 4.5,
                    MissCount = 2,
                    BattleTime = 60,
                    QuestionContents = "The quick brown fox jumps over the lazy dog",
                    InputContents = "The quick brown fox jumps over the lazy dog",
                    MissTypeContents = "[]",
                    BattleStatus = "completed"
                },
                new OnesideTwoPlayerTypingMatch
                {
                    UserId = users[1].Id,
                    EnemyUserId = users[3].Id,
                    Score = 1050,
                    Accuracy = 99.1,
                    TypeSpeed = 5.1,
                    MissCount = 1,
                    BattleTime = 60,
                    QuestionContents = "Programming is fun and challenging",
                    InputContents = "Programming is fun and challenging",
                    MissTypeContents = "[]",
                    BattleStatus = "completed"
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