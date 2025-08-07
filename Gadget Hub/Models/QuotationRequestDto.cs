namespace GadgetHub.WebAPI.Models;

public class QuotationRequestDto
{
    public required List<ProductOrderDto> ProductOrders { get; set; } = new();
    public required List<string> Distributors { get; set; } = new();
}

public class ProductOrderDto
{
    public required string ProductId { get; set; }
    public int QuantityRequested { get; set; }
}