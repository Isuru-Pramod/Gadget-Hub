using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service)
    {
        _service = service;
    }

    [HttpPut("cancel/{id}")]
    public IActionResult CancelOrder(Guid id)
    {
        var result = _service.CancelOrder(id);
        return result ? Ok("Order cancelled.") : NotFound("Order not found or already cancelled.");
    }


    [HttpPost("place")]
    public IActionResult PlaceOrder([FromBody] List<ProductOrder> orders)
    {
        var result = _service.PlaceOrder(orders);
        return Ok(result);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }
}
