using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User input)
    {
        var user = _service.Login(input.Username, input.Password);
        if (user == null)
            return Unauthorized("Invalid username or password.");

        return Ok(new
        {
            Message = "Login successful",
            Role = user.Role,
            UserId = user.Id
        });
    }
}
