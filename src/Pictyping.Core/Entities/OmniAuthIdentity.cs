namespace Pictyping.Core.Entities;

public class OmniAuthIdentity : BaseEntity
{
    public int UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;

    // Navigation properties
    public virtual User User { get; set; } = null!;
}