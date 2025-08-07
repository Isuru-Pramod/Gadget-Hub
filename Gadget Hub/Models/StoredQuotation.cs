using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetHub.WebAPI.Models;

public class StoredQuotation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Distributor { get; set; } = null!;

    [Required]
    public string ProductId { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int QuantityRequested { get; set; }

    [Required]
    public string Status { get; set; } = "Pending";

    [Precision(18, 2)]
    public decimal? PricePerUnit { get; set; }

    [Range(1, int.MaxValue)]
    public int? AvailableUnits { get; set; }

    [Range(1, int.MaxValue)]
    public int? EstimatedDeliveryDays { get; set; }

    [Required]
    public string CustomerUsername { get; set; } = null!;

    public Guid? OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order? Order { get; set; }
}