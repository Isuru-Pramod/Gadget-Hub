using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class AuthService
{
    private readonly List<User> _users = new()
    {
        new User { Username = "admin", Password = "admin123", Role = "admin" },
        new User { Username = "techworld", Password = "tech123", Role = "distributor" },
        new User { Username = "electrocom", Password = "electro123", Role = "distributor" },
        new User { Username = "gadgetcentral", Password = "gadget123", Role = "distributor" },
    };

    public User Login(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && u.Password == password);
    }
}
