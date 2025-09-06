namespace Pictyping.API.Models;

public class RankingItemDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Email { get; set; } = string.Empty;
}

