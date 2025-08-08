using GadgetHub.WebAPI.Models.Dtos;
using GadgetHub.WebAPI.Data;
using GadgetHub.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

    public bool AddResponse(QuotationResponse response)
    {
        var quotation = _db.Quotations.FirstOrDefault(q =>
            q.Distributor.ToLower() == response.Distributor.ToLower()
            && q.ProductId == response.ProductId
            && q.Status == "Pending");

        if (quotation == null) return false;

        quotation.PricePerUnit = response.PricePerUnit;
        quotation.AvailableUnits = response.AvailableUnits;
        quotation.EstimatedDeliveryDays = response.EstimatedDeliveryDays;
        quotation.Status = "Received";

        _db.SaveChanges(); // This saves to database
        return true;
    }

    public bool AddBatchResponses(BulkQuotationResponse batch)
    {
        var updatedAny = false;

        foreach (var response in batch.Responses)
        {
            var quotation = _db.Quotations.FirstOrDefault(q =>
                q.Distributor == batch.Distributor &&
                q.CustomerUsername == batch.CustomerUsername &&
                q.ProductId == response.ProductId &&
                q.Status == "Pending");

            if (quotation != null)
            {
                quotation.PricePerUnit = response.PricePerUnit;
                quotation.AvailableUnits = response.AvailableUnits;
                quotation.EstimatedDeliveryDays = response.EstimatedDeliveryDays;
                quotation.Status = "Received";
                updatedAny = true;
            }
        }

        if (updatedAny)
        {
            _db.SaveChanges();
        }

        return updatedAny;
    }

    public bool HandleBulkResponse(BulkQuotationResponse bulk)
    {
        bool allFound = true;

        foreach (var item in bulk.Responses)
        {
            var quote = _db.Quotations.FirstOrDefault(q =>
                q.Distributor == bulk.Distributor &&
                q.CustomerUsername == bulk.CustomerUsername &&
                q.ProductId == item.ProductId &&
                q.Status == "Pending");

            if (quote == null)
            {
                allFound = false;
                continue;
            }

            if (item.IsRejected)
            {
                quote.Status = "Rejected";
            }
            else
            {
                quote.PricePerUnit = item.PricePerUnit;
                quote.AvailableUnits = item.AvailableUnits;
                quote.EstimatedDeliveryDays = item.EstimatedDeliveryDays;
                quote.Status = "Received";
            }
        }

        _db.SaveChanges();
        return allFound;
    }

    public List<StoredQuotation> GetQuotationsByProduct(string productId)
    {
        return _db.Quotations.Where(q => q.ProductId == productId).ToList();
    }

    public List<StoredQuotation> GetAll() => _db.Quotations.ToList();

    public bool DeleteQuotation(int id)
    {
        var quotation = _db.Quotations.FirstOrDefault(q => q.Id == id);
        if (quotation == null) return false;

        _db.Quotations.Remove(quotation);
        _db.SaveChanges();
        return true;
    }

    public async Task<List<StoredQuotation>> GetQuotationsByCustomer(string customerUsername)
    {
        return await _db.Quotations
            .Where(q => q.CustomerUsername == customerUsername)
            .ToListAsync();
    }

    public async Task<List<StoredQuotation>> GetQuotationsByProductAndCustomer(string productId, string customerUsername)
    {
        return await _db.Quotations
            .Where(q => q.ProductId == productId && q.CustomerUsername == customerUsername)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}