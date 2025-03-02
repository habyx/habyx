using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using habyx.Data;
using habyx.Models;
using habyx.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace habyx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context,
            JwtService jwtService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting to register user: {model.Email}");

                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    _logger.LogWarning($"Registration failed: Email {model.Email} already exists");
                    return BadRequest(new { message = "Email already exists" }); // Changed to return JSON
                }

                var user = new User
                {
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedAt = DateTime.UtcNow // Ensure this is set
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Id} registered successfully");
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Registration failed for {model.Email}");
                return StatusCode(500, new 
                { 
                    message = "An error occurred while processing your request",
                    detail = ex.Message 
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginModel model) // Changed return type
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {model.Email}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Login failed for email: {model.Email}");
                    return Unauthorized(new { message = "Invalid email or password" }); // Changed to return JSON
                }

                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateToken(user);
                _logger.LogInformation($"User {user.Id} logged in successfully");
                
                // Return token in a JSON object
                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Login failed for {model.Email}");
                return StatusCode(500, new 
                { 
                    message = "An error occurred during login",
                    detail = ex.Message 
                });
            }
        }
    }
}