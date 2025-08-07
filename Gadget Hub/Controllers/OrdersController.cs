using GadgetHub.WebAPI.Data;
using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;
    private readonly ILogger<OrdersController> _logger;
    private readonly QuotationStore _quotationStore;
    private readonly AppDbContext _context;

    public OrdersController(
        OrderService service,
        ILogger<OrdersController> logger,
        QuotationStore quotationStore,
        AppDbContext context)
    {
        _service = service;
        _logger = logger;
        _quotationStore = quotationStore;
        _context = context;
    }

    [HttpPut("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        try
        {
            var result = await _service.CancelOrder(id);
            return result ? Ok("Order cancelled.") : NotFound("Order not found or already cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(500, "An error occurred while cancelling the order.");
        }
    }

    [HttpGet("customer/{username}/confirmed")]
    public async Task<IActionResult> GetCustomerConfirmedOrders(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username is required.");
            }

            var orders = await _service.GetCustomerConfirmedOrders(username);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting confirmed orders for customer {Username}", username);
            return StatusCode(500, "An error occurred while retrieving confirmed orders.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            var result = await _service.DeleteOrder(id);
            return result ? NoContent() : NotFound("Order not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order {OrderId}", id);
            return StatusCode(500, "An error occurred while deleting the order.");
        }
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrder([FromBody] List<ProductOrder> orders)
    {
        try
        {
            if (orders == null || !orders.Any())
            {
                return BadRequest("No products in order.");
            }

            var result = await _service.PlaceOrder(orders);
            return result == null
                ? BadRequest("Failed to place order")
                : Ok(new
                {
                    Message = "Order placed successfully",
                    Order = result
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error placing order");
            return StatusCode(500, "An error occurred while placing the order.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var orders = await _service.GetAllOrders();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all orders");
            return StatusCode(500, "An error occurred while retrieving orders.");
        }
    }

    [HttpGet("customer/{username}")]
    public async Task<IActionResult> GetCustomerOrders(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username is required.");
            }

            var orders = await _service.GetCustomerOrders(username);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for customer {Username}", username);
            return StatusCode(500, "An error occurred while retrieving customer orders.");
        }
    }

    [HttpGet("customer/{username}/all")]
    public async Task<IActionResult> GetAllCustomerOrders(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username is required.");
            }

            var orders = await _service.GetCustomerOrders(username);
            return Ok(orders.OrderByDescending(o => o.OrderDate).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all orders for customer {Username}", username);
            return StatusCode(500, "An error occurred while retrieving customer orders.");
        }
    }

    [HttpGet("customer/{username}/best-options")]
    public async Task<IActionResult> GetCustomerBestOptions(string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username is required.");
            }

            var bestOptions = await _service.GetCustomerBestOptions(username);
            return Ok(bestOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting best options for customer {Username}", username);
            return StatusCode(500, "An error occurred while retrieving best options.");
        }
    }


    [HttpPost("confirm-best/{productId}")]
    public async Task<IActionResult> ConfirmBestOption(string productId, [FromQuery] string username)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(productId))
            {
                return BadRequest("Username and ProductId are required.");
            }

            // Get all received quotations for this product and customer
            var quotations = await _quotationStore.GetQuotationsByProductAndCustomer(productId, username);
            var receivedQuotations = quotations.Where(q => q.Status == "Received").ToList();

            if (!receivedQuotations.Any())
            {
                return NotFound("No received quotations found for this product.");
            }

            // Find the best quotation (lowest price)
            var bestQuotation = receivedQuotations.OrderBy(q => q.PricePerUnit).First();

            // Create confirmed order
            var orderStatus = new OrderStatus
            {
                CustomerUsername = username,
                ProductId = productId,
                Quantity = bestQuotation.QuantityRequested,
                SelectedDistributor = bestQuotation.Distributor,
                PricePerUnit = bestQuotation.PricePerUnit ?? 0,
                EstimatedDeliveryDays = bestQuotation.EstimatedDeliveryDays ?? 0,
                Status = "Confirmed",
                OrderDate = DateTime.UtcNow
            };

            // Mark all quotations as processed
            foreach (var q in receivedQuotations)
            {
                q.Status = "Processed";
            }

            await _quotationStore.SaveChangesAsync();

            return Ok(new
            {
                Message = "Best option confirmed successfully",
                Order = orderStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming best option for product {ProductId}", productId);
            return StatusCode(500, "An error occurred while confirming best option.");
        }
    }
}