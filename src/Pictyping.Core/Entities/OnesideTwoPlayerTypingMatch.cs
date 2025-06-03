using System.Text.Json;

namespace Pictyping.Core.Entities;

public class OnesideTwoPlayerTypingMatch : BaseEntity
{
    public JsonDocument BattleDataJson { get; set; } = JsonDocument.Parse("{}");
    public string MatchId { get; set; } = string.Empty;
    public int RegisterId { get; set; }
    public int? EnemyId { get; set; }
    public int EnemyStartedRating { get; set; }
    public int StartedRating { get; set; }
    public bool IsFinished { get; set; } = false;
    public int? FinishedRating { get; set; }
    public int? BattleStatus { get; set; }

    // Navigation properties
    public virtual User RegisterUser { get; set; } = null!;
    public virtual User? EnemyUser { get; set; }
}