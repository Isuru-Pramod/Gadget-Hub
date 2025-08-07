using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<bool> EmailExistsAsync(string email)
    {
        bool staticExists = _staticUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        bool dbExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        return staticExists || dbExists;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        bool staticExists = _staticUsers.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        bool dbExists = await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        return staticExists || dbExists;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var staticUser = _staticUsers.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (staticUser != null)
            return staticUser;

        return await _context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == username.ToLower() &&
            u.Password == password);
    }

    public async Task<User> RegisterAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.Role = "customer";

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<User>> GetAllCustomersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}