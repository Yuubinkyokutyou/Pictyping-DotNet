namespace Pictyping.API.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsAdmin { get; set; }
}

