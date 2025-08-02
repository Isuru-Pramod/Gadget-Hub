using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class AuthService
{
    public bool EmailExists(string email)
    {
        return _staticUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) ||
               _registeredCustomers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    private readonly List<User> _staticUsers = new()
{
    new User { Username = "admin", Password = "admin123", Role = "admin", Email = "admin@gadgethub.com" },
    new User { Username = "techworld", Password = "123", Role = "distributor", Email = "tech@gadgethub.com" },
    new User { Username = "electrocom", Password = "123", Role = "distributor", Email = "electro@gadgethub.com" },
    new User { Username = "gadgetcentral", Password = "123", Role = "distributor", Email = "gadget@gadgethub.com" },
};

    private readonly List<User> _registeredCustomers = new(); // ✅ Make sure this is here!


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
        Console.WriteLine($"[SERVICE] Registering {user.Username} with email: {user.Email}");

        user.Id = Guid.NewGuid();
        user.Role = "customer";
        _registeredCustomers.Add(user); 

        return user;
    }


    public List<User> GetAllCustomers() => _registeredCustomers;
}
