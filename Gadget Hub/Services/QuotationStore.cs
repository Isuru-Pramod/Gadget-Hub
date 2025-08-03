using GadgetHub.WebAPI.Data;
using GadgetHub.WebAPI.Models;

public class QuotationStore
{
    private readonly AppDbContext _db;

    public QuotationStore(AppDbContext db)
    {
        _db = db;
    }

    public void CreateRequests(List<ProductOrder> orders, List<string> distributors, string customerUsername)
    {
        var newQuotations = new List<StoredQuotation>();

        foreach (var order in orders)
        {
            foreach (var distributor in distributors)
            {
                newQuotations.Add(new StoredQuotation
                {
                    Distributor = distributor,
                    ProductId = order.ProductId,
                    QuantityRequested = order.Quantity,
                    Status = "Pending",
                    CustomerUsername = customerUsername
                });
            }
        }

        _db.Quotations.AddRange(newQuotations);
        _db.SaveChanges();
    }

    public void AddResponse(QuotationResponse response)
    {
        var quotation = _db.Quotations.FirstOrDefault(q =>
            q.Distributor.Equals(response.Distributor, StringComparison.OrdinalIgnoreCase)
            && q.ProductId == response.ProductId);

        if (quotation != null)
        {
            quotation.PricePerUnit = response.PricePerUnit;
            quotation.AvailableUnits = response.AvailableUnits;
            quotation.EstimatedDeliveryDays = response.EstimatedDeliveryDays;
            quotation.Status = "Received";
            _db.SaveChanges();
        }
    }

    public List<StoredQuotation> GetQuotationsByProduct(string productId)
    {
        return _db.Quotations.Where(q => q.ProductId == productId).ToList();
    }

    public List<StoredQuotation> GetAll() => _db.Quotations.ToList();
}