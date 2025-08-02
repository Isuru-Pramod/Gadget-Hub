using GadgetHub.WebAPI.Models;

namespace GadgetHub.WebAPI.Services;

public class QuotationStore
{
    private readonly List<StoredQuotation> _store = new();

    public void CreateRequests(List<ProductOrder> orders, List<string> distributors)
    {
        foreach (var order in orders)
        {
            foreach (var distributor in distributors)
            {
                _store.Add(new StoredQuotation
                {
                    Distributor = distributor,
                    ProductId = order.ProductId,
                    QuantityRequested = order.Quantity,
                    Status = "Pending"
                });
            }
        }
    }

    public void AddResponse(QuotationResponse response)
    {
        var quotation = _store.FirstOrDefault(q =>
            q.Distributor.Equals(response.Distributor, StringComparison.OrdinalIgnoreCase)
            && q.ProductId == response.ProductId);

        if (quotation != null)
        {
            quotation.PricePerUnit = response.PricePerUnit;
            quotation.AvailableUnits = response.AvailableUnits;
            quotation.EstimatedDeliveryDays = response.EstimatedDeliveryDays;
            quotation.Status = "Received";
        }
    }

    public List<StoredQuotation> GetQuotationsByProduct(string productId)
    {
        return _store.Where(q => q.ProductId == productId).ToList();
    }

    public List<StoredQuotation> GetAll() => _store;
}
