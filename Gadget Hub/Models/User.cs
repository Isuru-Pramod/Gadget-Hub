namespace GadgetHub.WebAPI.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string Password { get; set; } // NOTE: In real apps, hash this!
    public string Role { get; set; } // "admin", "distributor"
}
