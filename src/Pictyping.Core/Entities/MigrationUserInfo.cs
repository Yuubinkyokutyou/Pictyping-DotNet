namespace Pictyping.Core.Entities;

public class MigrationUserInfo
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Provider { get; set; }
    public bool Admin { get; set; } = false;
    public int Rating { get; set; } = 1000;
    public DateTime CreatedAt { get; set; }
    public string? Jti { get; set; }
}