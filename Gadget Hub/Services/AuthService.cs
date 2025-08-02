using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class AuthService
{
    // Hardcoded users: admin + distributors
    private readonly List<User> _staticUsers = new()
    {
        new User { Username = "admin", Password = "admin123", Role = "admin" },
        new User { Username = "techworld", Password = "123", Role = "distributor" },
        new User { Username = "electrocom", Password = "123", Role = "distributor" },
        new User { Username = "gadgetcentral", Password = "123", Role = "distributor" },
    };

    // Dynamically registered customers
    private readonly List<User> _registeredCustomers = new();

    public User? Login(string username, string password)
    {
        return _staticUsers
            .Concat(_registeredCustomers)
            .FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password);
    }

    public bool UsernameExists(string username)
    {
        return _staticUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) ||
               _registeredCustomers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public User Register(User user)
    {
        user.Role = "customer";
        user.Id = Guid.NewGuid();
        _registeredCustomers.Add(user);
        return user;
    }

    public List<User> GetAllCustomers() => _registeredCustomers;
}
