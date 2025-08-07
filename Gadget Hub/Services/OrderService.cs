using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using GadgetHub.WebAPI.Models.Dtos;

namespace GadgetHub.WebAPI.Services;

public class OrderService
{
    private readonly AppDbContext _context;
    private readonly QuotationStore _quotationStore;

    public OrderService(AppDbContext context, QuotationStore quotationStore)
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
                Status = "Confirmed",
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

    public async Task<List<OrderStatus>> GetCustomerOrders(string username)
    {
        return await _context.OrderStatuses
            .Where(o => o.CustomerUsername == username)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> PlaceOrder(List<ProductOrder> productOrders)
    {
        var quotations = await _context.Quotations.ToListAsync();

        var selected = productOrders.Select(po =>
            quotations
                .Where(q => q.ProductId == po.ProductId && q.AvailableUnits >= po.Quantity)
                .OrderBy(q => q.PricePerUnit)
                .FirstOrDefault()
        ).Where(q => q != null).ToList();

        if (!selected.Any())
        {
            return null;
        }

        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Items = productOrders,
            SelectedQuotations = selected!,
            Status = "Confirmed",
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        await ProcessConfirmedOrders();

        return order;
    }

    public async Task<bool> CancelOrder(Guid orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order == null) return false;

        order.Status = "Cancelled";
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task ProcessCompletedQuotations(string customerUsername)
    {
        // Get all pending product orders for this customer
        var pendingProducts = await _context.Quotations
            .Where(q => q.CustomerUsername == customerUsername &&
                       q.Status == "Pending")
            .Select(q => q.ProductId)
            .Distinct()
            .ToListAsync();

        foreach (var productId in pendingProducts)
        {
            // Check if we have all 3 distributor responses
            var receivedCount = await _context.Quotations
                .CountAsync(q => q.ProductId == productId &&
                               q.CustomerUsername == customerUsername &&
                               q.Status == "Received");

            if (receivedCount == 3) // All 3 distributors responded
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

        // Find the best quotation (lowest price)
        var bestQuotation = receivedQuotations
            .OrderBy(q => q.PricePerUnit)
            .First();

        // Create confirmed order
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

        // Mark all quotations as processed
        foreach (var q in receivedQuotations)
        {
            q.Status = "Processed";
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteOrder(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null) return false;

        if (order.Items != null && order.Items.Any())
        {
            _context.RemoveRange(order.Items);
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Order>> GetAllOrders()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.SelectedQuotations)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<List<OrderStatus>> GetCustomerConfirmedOrders(string username)
    {
        return await _context.OrderStatuses
            .Where(o => o.CustomerUsername == username &&
                       o.Status == "Confirmed")
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<List<BestOptionDto>> GetCustomerBestOptions(string username)
    {
        var allQuotations = await _context.Quotations
            .Where(q => q.CustomerUsername == username)
            .ToListAsync();

        var bestOptions = allQuotations
            .Where(q => q.Status == "Received")
            .GroupBy(q => q.ProductId)
            .Select(group => new BestOptionDto
            {
                ProductId = group.Key,
                ProductName = string.Empty,
                Quantity = group.First().QuantityRequested,
                BestDistributor = group.OrderBy(q => q.PricePerUnit).First().Distributor,
                BestPrice = group.OrderBy(q => q.PricePerUnit).First().PricePerUnit ?? 0,
                EstimatedDeliveryDays = group.OrderBy(q => q.PricePerUnit).First().EstimatedDeliveryDays ?? 0,
                AllOptions = group.Select(q => new DistributorOptionDto
                {
                    DistributorName = q.Distributor,
                    Price = q.PricePerUnit ?? 0,
                    AvailableUnits = q.AvailableUnits ?? 0,
                    DeliveryDays = q.EstimatedDeliveryDays ?? 0
                }).ToList()
            })
            .ToList();

        var productIds = bestOptions.Select(b => b.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        foreach (var option in bestOptions)
        {
            var product = products.FirstOrDefault(p => p.Id == option.ProductId);
            if (product != null)
            {
                option.ProductName = product.Name;
                option.ProductImage = product.ImageData != null
                    ? $"data:{product.ImageType};base64,{Convert.ToBase64String(product.ImageData)}"
                    : null;
            }
        }

        return bestOptions;
    }
}