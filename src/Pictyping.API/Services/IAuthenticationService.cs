using Pictyping.Core.Entities;

namespace Pictyping.API.Services;

public interface IAuthenticationService
{
    Task<User?> ValidateUserAsync(string email, string password);
    Task<User> FindOrCreateUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int userId);
    Task SaveSessionAsync(string userId, string token);
    Task<string> GenerateTemporaryTokenAsync(string userId);
    
    // Domain Migration Strategy Implementation
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
}

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}

public interface ITypingBattleService
{
    Task<OnesideTwoPlayerTypingMatch> StartBattleAsync(int userId, int? enemyUserId);
    Task<OnesideTwoPlayerTypingMatch> FinishBattleAsync(int matchId, int score, double accuracy, double typeSpeed);
    Task<IEnumerable<OnesideTwoPlayerTypingMatch>> GetUserMatchesAsync(int userId);
}

public interface IRankingService
{
    Task<IEnumerable<User>> GetTopRankingsAsync(int count = 100);
    Task<int> GetUserRankAsync(int userId);
    Task UpdateUserRatingAsync(int userId, int newRating);
}