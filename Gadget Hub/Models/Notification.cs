using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class Notification
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Subject { get; set; } = null!;

    [Required]
    public string Message { get; set; } = null!;
}