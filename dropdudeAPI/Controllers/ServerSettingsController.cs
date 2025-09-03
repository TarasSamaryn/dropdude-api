using System;
using System.Threading.Tasks;
using DropDudeAPI.Data;
using DropDudeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DropDudeAPI.Controllers
{
    [ApiController]
    [Route("server-settings")]
    public class ServerSettingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ServerSettingsController> _logger;

        public ServerSettingsController(AppDbContext db, ILogger<ServerSettingsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET /server-settings
        [HttpGet]
        public async Task<IActionResult> GetServerSettings()
        {
            var settings = await _db.ServerGameSettings.FirstOrDefaultAsync();

            if (settings == null)
            {
                settings = CreateDefaultServerSettings();
                _db.ServerGameSettings.Add(settings);
                await _db.SaveChangesAsync();
                _logger.LogInformation("✅ Created default ServerGameSettings record");
                return Ok(settings);
            }

            // one-time backfill: якщо старий запис порожній після міграції — заповнюємо дефолтами
            bool isEmpty =
                settings.GameplayTimer == 0 &&
                settings.MaxPlayersForRandomRoom == 0 &&
                settings.MaxPlayersForRankedRoom == 0 &&
                settings.FindRoomSeconds == 0 &&
                settings.SkinsAmount == 0 &&
                (settings.FreeSkins == null || settings.FreeSkins.Length == 0) &&
                (settings.MonthlySkins == null || settings.MonthlySkins.Length == 0);

            if (isEmpty)
            {
                var defaults = CreateDefaultServerSettings();

                settings.GameplayTimer = defaults.GameplayTimer;
                settings.MaxPlayersForRandomRoom = defaults.MaxPlayersForRandomRoom;
                settings.MaxPlayersForRankedRoom = defaults.MaxPlayersForRankedRoom;

                settings.FindRoomSeconds = defaults.FindRoomSeconds;

                settings.SkinsAmount = defaults.SkinsAmount;
                settings.FreeSkins = defaults.FreeSkins;
                settings.MonthlySkins = defaults.MonthlySkins;

                await _db.SaveChangesAsync();
                _logger.LogInformation("🛠 Backfilled ServerGameSettings with defaults");
            }

            return Ok(settings);
        }

        // PUT /server-settings  (Admin only)
        [HttpPut]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> UpdateServerSettings([FromBody] ServerGameSettings input)
        {
            var settings = await _db.ServerGameSettings.FirstOrDefaultAsync();
            if (settings == null)
                return NotFound(new { error = "Server settings not found" });

            // Gameplay
            settings.GameplayTimer = input.GameplayTimer;
            settings.MaxPlayersForRandomRoom = input.MaxPlayersForRandomRoom;
            settings.MaxPlayersForRankedRoom = input.MaxPlayersForRankedRoom;

            // Server
            settings.FindRoomSeconds = input.FindRoomSeconds;

            // Skins
            settings.SkinsAmount = input.SkinsAmount;
            settings.FreeSkins = input.FreeSkins ?? Array.Empty<int>();
            settings.MonthlySkins = input.MonthlySkins ?? Array.Empty<int>();

            await _db.SaveChangesAsync();
            _logger.LogInformation("🔧 Server settings updated");

            return Ok(settings);
        }

        private static ServerGameSettings CreateDefaultServerSettings() => new ServerGameSettings
        {
            // Gameplay
            GameplayTimer = 300,
            MaxPlayersForRandomRoom = 6,
            MaxPlayersForRankedRoom = 6,

            // Server
            FindRoomSeconds = 30,

            // Skins
            SkinsAmount = 41,
            FreeSkins = new[] { 0, 6, 7 },
            MonthlySkins = new[] { 1, 2, 8 }
        };
    }
}
