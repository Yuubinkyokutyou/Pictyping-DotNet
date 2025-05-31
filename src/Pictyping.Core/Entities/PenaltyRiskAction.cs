namespace Pictyping.Core.Entities;

public class PenaltyRiskAction : BaseEntity
{
    public int ReporterId { get; set; }
    public string? PlayFabId { get; set; }
    public string? MatchId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public int? UserId { get; set; }
}