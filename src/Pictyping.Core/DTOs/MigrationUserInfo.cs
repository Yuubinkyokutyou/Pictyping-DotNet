namespace Pictyping.Core.DTOs;

/// <summary>
/// 移行ユーザー情報 (Domain Migration Strategy Implementation)
/// </summary>
public class MigrationUserInfo
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Provider { get; set; }
    public bool Admin { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Jti { get; set; }
}