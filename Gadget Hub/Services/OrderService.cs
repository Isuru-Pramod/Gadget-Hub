using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class OrderService
{
    private readonly List<Order> _orders = new();
    private readonly QuotationStore _quotationStore;

    public OrderService(QuotationStore quotationStore)
    {
        _quotationStore = quotationStore;
    }

    public bool CancelOrder(Guid orderId)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order != null && order.Status != "Cancelled")
        {
            order.Status = "Cancelled";
            return true;
        }
        return false;
    }


    public Order PlaceOrder(List<ProductOrder> productOrders)
    {
        var quotations = _quotationStore.GetAll();

        var selected = productOrders.Select(po =>
            quotations
                .Where(q => q.ProductId == po.ProductId && q.AvailableUnits >= po.Quantity)
                .OrderBy(q => q.PricePerUnit)
                .FirstOrDefault()
        ).Where(q => q != null).ToList();

        var order = new Order
        {
            Items = productOrders,
            SelectedQuotations = selected,
            Status = "Confirmed"
        };

        _orders.Add(order);
        return order;
    }

    public List<Order> GetAll() => _orders;
}
