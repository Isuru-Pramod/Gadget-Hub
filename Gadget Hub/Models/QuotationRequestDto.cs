namespace GadgetHub.WebAPI.Models;

public class QuotationRequestDto
{
    public List<ProductOrderDto> ProductOrders { get; set; }
    public List<string> Distributors { get; set; }
}

public class ProductOrderDto
{
    public string ProductId { get; set; }
    public int QuantityRequested { get; set; }
}
