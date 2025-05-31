namespace Pictyping.Core.Entities;

public class OnesideTwoPlayerTypingMatch : BaseEntity
{
    public int UserId { get; set; }
    public int? EnemyUserId { get; set; }
    public int Score { get; set; }
    public double Accuracy { get; set; }
    public double TypeSpeed { get; set; }
    public int MissCount { get; set; }
    public double BattleTime { get; set; }
    public string? QuestionContents { get; set; }
    public string? InputContents { get; set; }
    public string? MissTypeContents { get; set; }
    public string? BattleStatus { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual User? EnemyUser { get; set; }
}