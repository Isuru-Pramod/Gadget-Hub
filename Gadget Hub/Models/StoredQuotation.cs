using System.ComponentModel.DataAnnotations;

public class StoredQuotation
{
    [Key]
    public int Id { get; set; } // Primary Key

    public string Distributor { get; set; }
    public string ProductId { get; set; }
    public int QuantityRequested { get; set; }
    public string Status { get; set; }

    public decimal? PricePerUnit { get; set; }
    public int? AvailableUnits { get; set; }
    public int? EstimatedDeliveryDays { get; set; }

    public string CustomerUsername { get; set; }
}
