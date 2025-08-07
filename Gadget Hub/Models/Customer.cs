using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}