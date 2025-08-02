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
    public IActionResult Login([FromBody] LoginRequest input)
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
        try
        {
            Console.WriteLine($"[REGISTER] Username: {input.Username}, Email: {input.Email}");

            if (string.IsNullOrWhiteSpace(input.Username) ||
                string.IsNullOrWhiteSpace(input.Password) ||
                string.IsNullOrWhiteSpace(input.Email))
            {
                Console.WriteLine("Missing fields.");
                return BadRequest("Username, password, and email are required.");
            }

            if (!IsValidEmail(input.Email))
            {
                Console.WriteLine("Invalid email.");
                return BadRequest("Invalid email format.");
            }

            if (_service.UsernameExists(input.Username))
            {
                Console.WriteLine("Username exists.");
                return Conflict("Username already exists.");
            }

            if (_service.EmailExists(input.Email))
            {
                Console.WriteLine("Email exists.");
                return Conflict("Email already in use.");
            }

            var user = _service.Register(input);
            Console.WriteLine($"[REGISTER] Success for {user.Username}");

            return Ok(new
            {
                message = "Customer registered successfully",
                username = user.Username,
                role = user.Role,
                userId = user.Id
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Registration failed: {ex.Message}");
            return StatusCode(500, "Server error: " + ex.Message);
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }



}
