using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}