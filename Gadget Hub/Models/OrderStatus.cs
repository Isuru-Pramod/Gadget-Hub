using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetHub.WebAPI.Models;

public class OrderStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string CustomerUsername { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
    public string SelectedDistributor { get; set; } = null!;
    public decimal PricePerUnit { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public string Status { get; set; } = null!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}