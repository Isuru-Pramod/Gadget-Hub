using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class QuotationRequest
{
    [Required, MinLength(1)]
    public List<ProductOrder> ProductOrders { get; set; } = new();

    [Required, MinLength(1)]
    public List<string> Distributors { get; set; } = new();

    [Required]
    public string CustomerUsername { get; set; } = null!;
}