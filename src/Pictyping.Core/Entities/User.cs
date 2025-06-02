namespace Pictyping.Core.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public DateTime? RememberCreatedAt { get; set; }
    public bool Guest { get; set; } = false;
    public string? PlayFabId { get; set; }
    public int Rating { get; set; } = 1200;
    public bool Admin { get; set; } = false;
    public string? DisplayName { get; set; }

    // Navigation properties
    public virtual ICollection<OnesideTwoPlayerTypingMatch> TypingMatchesAsPlayer { get; set; } = new List<OnesideTwoPlayerTypingMatch>();
    public virtual ICollection<OnesideTwoPlayerTypingMatch> TypingMatchesAsEnemy { get; set; } = new List<OnesideTwoPlayerTypingMatch>();
    public virtual ICollection<OmniAuthIdentity> OmniAuthIdentities { get; set; } = new List<OmniAuthIdentity>();
}