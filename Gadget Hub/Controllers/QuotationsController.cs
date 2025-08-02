using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationsController : ControllerBase
{
    private readonly QuotationStore _store;

    public QuotationsController(QuotationStore store)
    {
        _store = store;
    }

    // Called by frontend when customer places an order
    [HttpPost("request")]
    public IActionResult RequestQuotations([FromBody] QuotationRequestDto request)
    {
        if (request.ProductOrders == null || request.Distributors == null)
        {
            return BadRequest("Missing product orders or distributor list.");
        }

        _store.CreateRequests(request.ProductOrders, request.Distributors);
        return Ok("Quotation requests created. Awaiting distributor responses.");
    }

    // Called manually by distributor (simulate via Swagger)
    [HttpPost("respond")]
    public IActionResult RespondToQuotation([FromBody] QuotationResponse response)
    {
        _store.AddResponse(response);
        return Ok("Quotation response saved.");
    }

    // For viewing quotation status per product
    [HttpGet("status")]
    public IActionResult GetStatus([FromQuery] string productId)
    {
        return Ok(_store.GetQuotationsByProduct(productId));
    }

    // View all quotation requests (for admin/debug)
    [HttpGet("all")]
    public IActionResult GetAll() => Ok(_store.GetAll());
}
