using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebAPI.Services;

public class OrderProcessingService
{
    private readonly AppDbContext _context;
    private readonly QuotationStore _quotationStore;

    public OrderProcessingService(AppDbContext context, QuotationStore quotationStore)
    {
        _context = context;
        _quotationStore = quotationStore;
    }

    public async Task ProcessConfirmedOrders()
    {
        var receivedQuotations = await _context.Quotations
            .Where(q => q.Status == "Received")
            .ToListAsync();

        var grouped = receivedQuotations
            .GroupBy(q => new { q.ProductId, q.CustomerUsername });

        foreach (var group in grouped)
        {
            var bestQuotation = group.OrderBy(q => q.PricePerUnit).First();

            var orderStatus = new OrderStatus
            {
                CustomerUsername = bestQuotation.CustomerUsername,
                ProductId = bestQuotation.ProductId,
                Quantity = bestQuotation.QuantityRequested,
                SelectedDistributor = bestQuotation.Distributor,
                PricePerUnit = bestQuotation.PricePerUnit ?? 0,
                EstimatedDeliveryDays = bestQuotation.EstimatedDeliveryDays ?? 0,
                Status = "Confirmed3"
            };

            _context.OrderStatuses.Add(orderStatus);

            foreach (var q in group)
            {
                q.Status = "Processed";
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderStatus>> GetCustomerOrders(string username)
    {
        return await _context.OrderStatuses
            .Where(o => o.CustomerUsername == username)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}