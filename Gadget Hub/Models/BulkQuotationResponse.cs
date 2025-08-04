namespace GadgetHub.WebAPI.Models.Dtos
{
    public class BulkQuotationResponse
    {
        public string Distributor { get; set; } = string.Empty;
        public string CustomerUsername { get; set; } = string.Empty;
        public List<QuotationResponseItem> Responses { get; set; } = new();
    }

    public class QuotationResponseItem
    {
        public string ProductId { get; set; } = string.Empty;
        public bool IsRejected { get; set; } = false;

        // Only required if not rejected
        public decimal? PricePerUnit { get; set; }
        public int? AvailableUnits { get; set; }
        public int? EstimatedDeliveryDays { get; set; }
    }
}
