using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class ProductUploadDto
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Category { get; set; } = null!;

    [Required]
    public IFormFile Image { get; set; } = null!;
}