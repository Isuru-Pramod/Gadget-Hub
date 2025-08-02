namespace GadgetHub.WebAPI.Models;

public class QuotationRequest
{
    public List<ProductOrder> ProductOrders { get; set; }
    public List<string> Distributors { get; set; }
}
