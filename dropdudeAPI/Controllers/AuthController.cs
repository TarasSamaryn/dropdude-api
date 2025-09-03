using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DropDudeAPI.Data;
using DropDudeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DropDudeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext db, IConfiguration config, ILogger<AuthController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("🔔 Register endpoint hit");
            _logger.LogInformation("Payload: {@Req}", request);

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username cannot be empty.");
            }

            if (_db.Players.Any(p => p.Username == request.Username))
            {
                return BadRequest("User with this username already exists.");
            }

            // ✅ Беремо дефолтні безкоштовні скіни з ServerGameSettings (створюємо, якщо нема)
            var serverSettings = _db.ServerGameSettings.FirstOrDefault();
            if (serverSettings == null)
            {
                serverSettings = new ServerGameSettings
                {
                    FreeSkins = new[] { 0, 6, 7 }
                };
                _db.ServerGameSettings.Add(serverSettings);
                _db.SaveChanges();
            }

            var freeSkins = serverSettings.FreeSkins ?? Array.Empty<int>();
            string freeSkinsRaw = string.Join(",", freeSkins);

            Player player = new Player
            {
                Username         = request.Username,
                PasswordHash     = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt        = DateTimeOffset.UtcNow,
                IsAdmin          = false,
                BodiesSkins      = freeSkinsRaw,
                LastSelectedSkin = freeSkins.Length > 0 ? freeSkins[0].ToString() : "0"
            };

            _db.Players.Add(player);
            _db.SaveChanges();

            _logger.LogInformation("✅ User {Username} registered successfully", player.Username);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("🔔 Login attempt: {Username}", request.Username);

            try
            {
                Player? player = _db.Players.FirstOrDefault(p => p.Username == request.Username);
                if (player == null)
                {
                    _logger.LogWarning("❌ Login failed – user not found: {Username}", request.Username);
                    return Unauthorized("Invalid username or password.");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, player.PasswordHash))
                {
                    _logger.LogWarning("❌ Login failed – wrong password: {Username}", request.Username);
                    return Unauthorized("Invalid username or password.");
                }

                byte[] key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, player.Id.ToString()),
                        new Claim(ClaimTypes.Name, player.Username),
                        new Claim("isAdmin", player.IsAdmin.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
                string? jwt = tokenHandler.WriteToken(token);

                _logger.LogInformation("✅ User {Username} logged in, JWT issued", player.Username);
                return Ok(new
                {
                    token = jwt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception during login for {Username}: {Message}", request.Username, ex.Message);
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
