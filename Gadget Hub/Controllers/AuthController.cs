using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
            {
                _logger.LogWarning("Login attempt with missing credentials");
                return BadRequest("Username and password are required.");
            }

            var user = await _service.LoginAsync(input.Username, input.Password);
            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", input.Username);
                return Unauthorized("Invalid username or password.");
            }

            _logger.LogInformation("User {Username} logged in successfully", user.Username);
            return Ok(new
            {
                message = "Login successful",
                username = user.Username,
                role = user.Role?.ToLower(),
                userId = user.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for username: {Username}", input.Username);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User input)
    {
        try
        {
            _logger.LogInformation("Registration attempt for username: {Username}, email: {Email}",
                input.Username, input.Email);

            // Validate input
            if (string.IsNullOrWhiteSpace(input.Username) ||
                string.IsNullOrWhiteSpace(input.Password) ||
                string.IsNullOrWhiteSpace(input.Email))
            {
                _logger.LogWarning("Registration failed - missing required fields");
                return BadRequest("Username, password, and email are required.");
            }

            if (!IsValidEmail(input.Email))
            {
                _logger.LogWarning("Registration failed - invalid email format: {Email}", input.Email);
                return BadRequest("Invalid email format.");
            }

            // Check for existing user
            if (await _service.UsernameExistsAsync(input.Username))
            {
                _logger.LogWarning("Registration failed - username already exists: {Username}", input.Username);
                return Conflict("Username already exists.");
            }

            if (await _service.EmailExistsAsync(input.Email))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", input.Email);
                return Conflict("Email already in use.");
            }

            // Register user
            var user = await _service.RegisterAsync(input);
            if (user == null)
            {
                _logger.LogError("Registration failed - user creation returned null");
                return StatusCode(500, "Failed to create user.");
            }

            _logger.LogInformation("User registered successfully: {Username}", user.Username);
            return Ok(new
            {
                message = "Registration successful",
                username = user.Username,
                role = user.Role,
                userId = user.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for username: {Username}", input.Username);
            return StatusCode(500, "An error occurred during registration.");
        }
    }

    private static bool IsValidEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }
}