namespace GadgetHub.WebAPI.Models.Dtos
{
    public class BulkQuotationResponse
    {
        public required string Distributor { get; set; }
        public required string CustomerUsername { get; set; }
        public required List<QuotationResponseItem> Responses { get; set; } = new();
    }

    public class QuotationResponseItem
    {
        public required string ProductId { get; set; }
        public bool IsRejected { get; set; } = false;
        public decimal? PricePerUnit { get; set; }
        public int? AvailableUnits { get; set; }
        public int? EstimatedDeliveryDays { get; set; }
    }
}