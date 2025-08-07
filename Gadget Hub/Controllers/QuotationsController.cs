using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using GadgetHub.WebAPI.Models.Dtos;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationsController : ControllerBase
{
    private readonly QuotationStore _store;
    private readonly OrderService _orderService;

    public QuotationsController(QuotationStore store, OrderService orderService)
    {
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

    [HttpPost("respond")]
    public async Task<IActionResult> RespondToQuotation([FromBody] QuotationResponse response)
    {
        bool success = _store.AddResponse(response);
        if (!success)
        {
            return NotFound("Pending quotation not found for the specified distributor and product.");
        }

        await _orderService.ProcessConfirmedOrders();
        return Ok("Quotation response saved successfully.");
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

    [HttpDelete("{id}")]
    public IActionResult DeleteQuotation(int id)
    {
        bool success = _store.DeleteQuotation(id);
        return success
            ? Ok("Quotation deleted successfully")
            : NotFound("Quotation not found");
    }
}