using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebAPI.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    private readonly List<User> _staticUsers = new()
    {
        new User { Username = "admin", Password = "admin123", Role = "admin", Email = "admin@gadgethub.com" },
        new User { Username = "techworld", Password = "123", Role = "distributor", Email = "tech@gadgethub.com" },
        new User { Username = "electrocom", Password = "123", Role = "distributor", Email = "electro@gadgethub.com" },
        new User { Username = "gadgetcentral", Password = "123", Role = "distributor", Email = "gadget@gadgethub.com" },
    };

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public bool EmailExists(string email)
    {
        return _staticUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) ||
               _context.Users.Any(u => u.Email.ToLower() == email.ToLower());
    }

    public bool UsernameExists(string username)
    {
        return _staticUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) ||
               _context.Users.Any(u => u.Username.ToLower() == username.ToLower());
    }

    public User? Login(string username, string password)
    {
        var staticUser = _staticUsers.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);

        if (staticUser != null) return staticUser;

        return _context.Users.FirstOrDefault(u =>
            u.Username.ToLower() == username.ToLower() && u.Password == password);
    }

    public User Register(User user)
    {
        user.Id = Guid.NewGuid();
        user.Role = "customer";

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public List<User> GetAllCustomers()
    {
        return _context.Users.ToList();
    }
}
