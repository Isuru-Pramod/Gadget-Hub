using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Models;

public class QuotationResponse
{
    [Required]
    public string Distributor { get; set; } = null!;

    [Required]
    public string ProductId { get; set; } = null!;

    [Range(0.01, double.MaxValue)]
    public decimal PricePerUnit { get; set; }

    [Range(1, int.MaxValue)]
    public int AvailableUnits { get; set; }

    [Range(1, int.MaxValue)]
    public int EstimatedDeliveryDays { get; set; }

    [Required]
    public string CustomerUsername { get; set; } = null!;
}