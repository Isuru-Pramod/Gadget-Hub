namespace GadgetHub.WebAPI.Models;

public class StoredQuotation
{
    public string Distributor { get; set; }
    public string ProductId { get; set; }
    public int QuantityRequested { get; set; }
    public decimal? PricePerUnit { get; set; }
    public int? AvailableUnits { get; set; }
    public int? EstimatedDeliveryDays { get; set; }
    public string Status { get; set; } = "Pending";
}
