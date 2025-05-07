using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MinefieldServer.Data;

namespace MinefieldServer.Controllers
{
    [ApiController]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(AppDbContext db, ILogger<ProfileController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetProfile()
        {
            _logger.LogInformation("📥 GET /profile requested");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var id))
            {
                _logger.LogWarning("⛔ Failed to parse user ID from token ({Token})", userId);
                return Unauthorized(new { error = "Invalid token" });
            }

            var player = _db.Players.Find(id);
            if (player == null)
            {
                _logger.LogWarning("❌ Player with ID {Id} not found", id);
                return NotFound(new { error = "Player not found" });
            }

            _logger.LogInformation("✅ Profile returned for player '{Username}' (ID: {Id})", player.Username, id);
            return Ok(new
            {
                username  = player.Username,
                testCoins = player.TestCoins,
                realCoins = player.RealCoins,
                isAdmin   = player.IsAdmin
            });
        }
    }
}