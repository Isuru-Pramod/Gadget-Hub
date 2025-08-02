public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? FullName { get; set; }
    public string Email { get; set; } = null!;  
}
