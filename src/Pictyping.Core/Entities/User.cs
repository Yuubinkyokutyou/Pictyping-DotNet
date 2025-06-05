namespace Pictyping.Core.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordSentAt { get; set; }
    public DateTime? RememberCreatedAt { get; set; }
    public bool Guest { get; set; } = false;
    public string? PlayfabId { get; set; }  // Rails schema uses "playfabId" 
    public string Name { get; set; } = "noname";  // Rails schema has "name"
    public int Rating { get; set; } = 1200;
    public DateTime? OnlineGameBanDate { get; set; }  // Rails schema has "online_game_ban_date"
    public bool Admin { get; set; } = false;
    public string? Provider { get; set; }  // OAuth provider (google, etc.) for migration compatibility

    // Navigation properties
    public virtual ICollection<OnesideTwoPlayerTypingMatch> TypingMatchesAsRegister { get; set; } = new List<OnesideTwoPlayerTypingMatch>();
    public virtual ICollection<OnesideTwoPlayerTypingMatch> TypingMatchesAsEnemy { get; set; } = new List<OnesideTwoPlayerTypingMatch>();
    public virtual ICollection<OmniAuthIdentity> OmniAuthIdentities { get; set; } = new List<OmniAuthIdentity>();
    public virtual ICollection<PenaltyRiskAction> PenaltyRiskActions { get; set; } = new List<PenaltyRiskAction>();
}