namespace GadgetHub.WebAPI.Models.Dtos
{
    public class BestOptionDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string BestDistributor { get; set; } = string.Empty;
        public decimal BestPrice { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public string? OrderId { get; set; }
        public List<DistributorOptionDto> AllOptions { get; set; } = new();
    }

    public class DistributorOptionDto
    {
        public string DistributorName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableUnits { get; set; }
        public int DeliveryDays { get; set; }
        public string? OrderId { get; set; }
    }
}