using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using GadgetHub.WebAPI.Models.Dtos;

namespace GadgetHub.WebAPI.Services;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    // Frontend-required methods
    public async Task<List<OrderStatus>> GetCustomerOrders(string username)
    {
        return await _context.OrderStatuses
            .Where(o => o.CustomerUsername == username)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<List<OrderStatus>> GetAllOrderStatuses()
    {
        return await _context.OrderStatuses
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }


    public async Task<List<OrderStatus>> GetCustomerConfirmedOrders(string username)
    {
        return await _context.OrderStatuses
            .Where(o => o.CustomerUsername == username && o.Status == "Confirmed")
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<bool> DeleteOrderStatus(int id)
    {
        var status = await _context.OrderStatuses.FindAsync(id);
        if (status == null) return false;

        _context.OrderStatuses.Remove(status);
        await _context.SaveChangesAsync();
        return true;
    }

    // Background service required methods
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
                Status = "ConfirmedOLD",
                OrderDate = DateTime.UtcNow
            };

            _context.OrderStatuses.Add(orderStatus);

            foreach (var q in group)
            {
                q.Status = "Processed";
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task ProcessCompletedQuotations(string customerUsername)
    {
        var pendingProducts = await _context.Quotations
            .Where(q => q.CustomerUsername == customerUsername && q.Status == "Pending")
            .Select(q => q.ProductId)
            .Distinct()
            .ToListAsync();

        foreach (var productId in pendingProducts)
        {
            var receivedCount = await _context.Quotations
                .CountAsync(q => q.ProductId == productId &&
                               q.CustomerUsername == customerUsername &&
                               q.Status == "Received");

            if (receivedCount >= 3) // At least 3 distributor responses
            {
                await ProcessConfirmedOrdersForProduct(customerUsername, productId);
            }
        }
    }

    private async Task ProcessConfirmedOrdersForProduct(string customerUsername, string productId)
    {
        var receivedQuotations = await _context.Quotations
            .Where(q => q.ProductId == productId &&
                       q.CustomerUsername == customerUsername &&
                       q.Status == "Received")
            .ToListAsync();

        if (!receivedQuotations.Any()) return;

        var bestQuotation = receivedQuotations
            .OrderBy(q => q.PricePerUnit)
            .First();

        var orderStatus = new OrderStatus
        {
            CustomerUsername = customerUsername,
            ProductId = productId,
            Quantity = bestQuotation.QuantityRequested,
            SelectedDistributor = bestQuotation.Distributor,
            PricePerUnit = bestQuotation.PricePerUnit ?? 0,
            EstimatedDeliveryDays = bestQuotation.EstimatedDeliveryDays ?? 0,
            Status = "Confirmed",
            OrderDate = DateTime.UtcNow
        };

        _context.OrderStatuses.Add(orderStatus);

        foreach (var q in receivedQuotations)
        {
            q.Status = "Processed";
        }

        await _context.SaveChangesAsync();
    }
}