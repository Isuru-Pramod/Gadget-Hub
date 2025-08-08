using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class ProductOrder
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ProductId { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}