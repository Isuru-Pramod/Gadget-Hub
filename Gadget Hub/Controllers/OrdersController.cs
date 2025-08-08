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

    [HttpGet("order-statuses")]
    public async Task<IActionResult> GetAllOrderStatuses()
    {
        var orders = await _service.GetAllOrderStatuses();
        return Ok(orders);
    }

    [HttpGet("customer/{username}")]
    public async Task<IActionResult> GetCustomerOrders(string username)
    {
        var orders = await _service.GetCustomerOrders(username);
        return Ok(orders);
    }

    [HttpGet("customer/{username}/confirmed")]
    public async Task<IActionResult> GetCustomerConfirmedOrders(string username)
    {
        var orders = await _service.GetCustomerConfirmedOrders(username);
        return Ok(orders);
    }

    [HttpDelete("order-statuses/{id}")]
    public async Task<IActionResult> DeleteOrderStatus(int id)
    {
        var result = await _service.DeleteOrderStatus(id);
        return result ? Ok() : NotFound();
    }
}