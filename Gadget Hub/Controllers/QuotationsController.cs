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

    [HttpPost("request")]
    public IActionResult RequestQuotations([FromBody] QuotationRequest request)
    {
        _store.CreateRequests(request.ProductOrders, request.Distributors);
        return Ok("Quotation requests created. Awaiting distributor responses.");
    }

    [HttpPost("respond")]
    public IActionResult RespondToQuotation([FromBody] QuotationResponse response)
    {
        _store.AddResponse(response);
        return Ok("Quotation response saved.");
    }

    [HttpGet("status")]
    public IActionResult GetStatus([FromQuery] string productId)
    {
        return Ok(_store.GetQuotationsByProduct(productId));
    }

    [HttpGet("all")]
    public IActionResult GetAll() => Ok(_store.GetAll());
}
