namespace GadgetHub.WebAPI.Models
{
    public class QuotationResponse
    {
        public string Distributor { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public decimal PricePerUnit { get; set; }
        public int AvailableUnits { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public string CustomerUsername { get; set; } = string.Empty;
    }
}