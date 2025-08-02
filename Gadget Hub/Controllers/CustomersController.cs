using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _service;

    public CustomersController(CustomerService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public IActionResult Register(Customer customer)
    {
        return Ok(_service.Register(customer));
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Customer input)
    {
        var result = _service.Login(input.Email, input.Password);
        return result == null ? Unauthorized("Invalid credentials") : Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var customer = _service.GetById(id);
        return customer == null ? NotFound() : Ok(customer);
    }
}
