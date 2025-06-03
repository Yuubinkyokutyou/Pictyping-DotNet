namespace Pictyping.Core.Entities;

public class OmniAuthIdentity : BaseEntity
{
    public string? Provider { get; set; }
    public string? Uid { get; set; }
    public string? Email { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}