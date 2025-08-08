using GadgetHub.WebAPI.Data;
using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Models.Dtos;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationsController : ControllerBase
{
    private readonly QuotationStore _store;
    private readonly OrderService _orderService;
    private readonly AppDbContext _context;
    private readonly ILogger<QuotationsController> _logger;

    public QuotationsController(
        AppDbContext context,
        ILogger<QuotationsController> logger,
        QuotationStore store,
        OrderService orderService)
    {
        _context = context;
        _logger = logger;
        _store = store;
        _orderService = orderService;
    }

    [HttpGet("customer-orders")]
    public async Task<IActionResult> GetCustomerOrders([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required.");
        }

        var orders = await _orderService.GetCustomerOrders(username);
        return Ok(orders);
    }

    [HttpPost("request")]
    public IActionResult RequestQuotations([FromBody] QuotationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerUsername))
        {
            return BadRequest("Customer username is required.");
        }

        if (request.ProductOrders == null || request.ProductOrders.Count == 0)
        {
            return BadRequest("No product orders found.");
        }

        _store.CreateRequests(request.ProductOrders, request.Distributors, request.CustomerUsername);
        return Ok("Quotation requests created. Awaiting distributor responses.");
    }

    [HttpGet("all-quotations")]
    public async Task<IActionResult> GetAllQuotations()
    {
        try
        {
            var quotations = await _context.Quotations
                .OrderByDescending(q => q.Id)
                .ToListAsync();

            return Ok(quotations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all quotations");
            return StatusCode(500, "An error occurred while fetching quotations");
        }
    }

    [HttpDelete("delete-quotation/{id}")]
    public async Task<IActionResult> DeleteQuotation(int id)
    {
        try
        {
            var quotation = await _context.Quotations.FindAsync(id);
            if (quotation == null)
            {
                return NotFound($"Quotation with ID {id} not found");
            }

            _context.Quotations.Remove(quotation);
            await _context.SaveChangesAsync();

            return Ok($"Quotation with ID {id} deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quotation with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the quotation");
        }
    }


    [HttpPost("respond-batch")]
    public async Task<IActionResult> RespondBatch([FromBody] BulkQuotationResponse bulk)
    {
        if (string.IsNullOrWhiteSpace(bulk.Distributor) ||
            string.IsNullOrWhiteSpace(bulk.CustomerUsername))
        {
            return BadRequest("Distributor and CustomerUsername are required.");
        }

        var success = _store.HandleBulkResponse(bulk);

        if (!success)
        {
            return NotFound("Some quotations were not found or already processed.");
        }

        await _orderService.ProcessCompletedQuotations(bulk.CustomerUsername);
        return Ok("Responses processed successfully.");
    }

    [HttpGet("status")]
    public IActionResult GetStatus([FromQuery] string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return BadRequest("Product ID is required.");

        var result = _store.GetQuotationsByProduct(productId);
        return Ok(result);
    }

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        return Ok(_store.GetAll());
    }


}