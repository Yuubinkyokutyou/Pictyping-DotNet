using Pictyping.Core.Entities;

namespace Pictyping.Core.Interfaces;

public interface ITypingBattleService
{
    Task<OnesideTwoPlayerTypingMatch?> GetMatchByIdAsync(int id);
    Task<OnesideTwoPlayerTypingMatch> CreateMatchAsync(int userId);
    Task<OnesideTwoPlayerTypingMatch> UpdateMatchAsync(OnesideTwoPlayerTypingMatch match);
    Task<IEnumerable<OnesideTwoPlayerTypingMatch>> GetUserMatchesAsync(int userId);
    Task<OnesideTwoPlayerTypingMatch?> GetActiveMatchAsync(int userId);
}