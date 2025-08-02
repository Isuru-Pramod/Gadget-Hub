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
        if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var user = _service.Login(input.Username, input.Password);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(new
        {
            message = "Login successful",
            username = user.Username,
            role = user.Role.ToLower(),
            userId = user.Id
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User input)
    {
        if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
        {
            return BadRequest("Username and password are required.");
        }

        if (_service.UsernameExists(input.Username))
        {
            return Conflict("Username already exists.");
        }

        var user = _service.Register(input);

        return Ok(new
        {
            message = "Customer registered successfully",
            username = user.Username,
            role = user.Role,
            userId = user.Id
        });
    }
}
