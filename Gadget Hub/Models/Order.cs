
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GadgetHub.WebAPI.Models;

public class Order
{
    [Key]
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public List<ProductOrder> Items { get; set; } = new();
    public List<StoredQuotation> SelectedQuotations { get; set; } = new();
    public string Status { get; set; } = "Processing";
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}