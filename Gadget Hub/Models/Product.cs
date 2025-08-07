using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class Product
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Category { get; set; } = null!;

    public byte[]? ImageData { get; set; }
    public string? ImageType { get; set; }
}