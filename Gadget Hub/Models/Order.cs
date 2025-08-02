namespace GadgetHub.WebAPI.Models;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public List<ProductOrder> Items { get; set; }
    public string Status { get; set; } = "Processing";
    public List<StoredQuotation> SelectedQuotations { get; set; } // Changed from List<QuotationResponse>
}