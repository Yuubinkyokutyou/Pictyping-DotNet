namespace Pictyping.Core.Entities;

public class PenaltyRiskAction : BaseEntity
{
    public int UserId { get; set; }
    public int ActionType { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}