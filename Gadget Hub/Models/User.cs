using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string Role { get; set; } = null!;

    public string? FullName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = null!;
}