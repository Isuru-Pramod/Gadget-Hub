using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageType { get; set; }
}
