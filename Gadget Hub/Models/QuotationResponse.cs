namespace GadgetHub.WebAPI.Models;

public class QuotationResponse
{
    public string Distributor { get; set; }
    public string ProductId { get; set; }
    public decimal PricePerUnit { get; set; }
    public int AvailableUnits { get; set; }
    public int EstimatedDeliveryDays { get; set; }
}
