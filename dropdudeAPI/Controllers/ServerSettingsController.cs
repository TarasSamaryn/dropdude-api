using System;
using System.Linq;
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

            // backfill якщо нові поля порожні
            if (settings.MediumRatingMin == 0 && settings.MediumRatingMax == 0)
            {
                var d = CreateDefaultServerSettings();
                settings.MediumRatingMin = d.MediumRatingMin;
                settings.MediumRatingMax = d.MediumRatingMax;
                await _db.SaveChangesAsync();
                _logger.LogInformation("🛠 Backfilled ServerGameSettings with bot difficulty defaults");
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

            int validatedGameplayTimer   = ClampNonNegative(input.GameplayTimer);
            int validatedMaxPlayersRandom = ClampAtLeast(input.MaxPlayersForRandomRoom, 1);
            int validatedMaxPlayersRanked = ClampAtLeast(input.MaxPlayersForRankedRoom, 1);
            int validatedFindRoomSeconds = ClampNonNegative(input.FindRoomSeconds);
            int validatedSkinsAmount     = ClampNonNegative(input.SkinsAmount);

            // нові поля
            if (input.MediumRatingMax < input.MediumRatingMin)
                return BadRequest("MediumRatingMax must be >= MediumRatingMin.");

            settings.GameplayTimer = validatedGameplayTimer;
            settings.MaxPlayersForRandomRoom = validatedMaxPlayersRandom;
            settings.MaxPlayersForRankedRoom = validatedMaxPlayersRanked;
            settings.FindRoomSeconds = validatedFindRoomSeconds;
            settings.SkinsAmount = validatedSkinsAmount;

            settings.FreeSkins = SanitizeSkinArray(input.FreeSkins, settings.SkinsAmount);
            settings.MonthlySkins = SanitizeSkinArray(input.MonthlySkins, settings.SkinsAmount);

            settings.MediumRatingMin = input.MediumRatingMin; // NEW
            settings.MediumRatingMax = input.MediumRatingMax; // NEW

            await _db.SaveChangesAsync();
            _logger.LogInformation("🔧 Server settings updated (validated)");

            return Ok(settings);
        }

        // NEW endpoint: визначення рівня бота
        [HttpGet("bots/difficulty")]
        public async Task<IActionResult> GetBotDifficulty([FromQuery] double rating)
        {
            var s = await _db.ServerGameSettings.FirstOrDefaultAsync() ?? CreateDefaultServerSettings();
            string diff = rating < s.MediumRatingMin ? "Easy"
                        : rating > s.MediumRatingMax ? "Hard"
                        : "Medium";

            return Ok(new { rating, difficulty = diff });
        }

        private static int ClampNonNegative(int v) => v < 0 ? 0 : v;
        private static int ClampAtLeast(int v, int min) => v < min ? min : v;

        private static int[] SanitizeSkinArray(int[]? arr, int skinsAmount)
        {
            if (arr == null) return Array.Empty<int>();

            return arr
                .Where(x => x >= 0 && x < skinsAmount)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
        }

        private static ServerGameSettings CreateDefaultServerSettings() => new ServerGameSettings
        {
            GameplayTimer = 300,
            MaxPlayersForRandomRoom = 6,
            MaxPlayersForRankedRoom = 6,
            FindRoomSeconds = 30,
            SkinsAmount = 41,
            FreeSkins = new[] { 0, 6, 7 },
            MonthlySkins = new[] { 1, 2, 8 },

            MediumRatingMin = 800,   // NEW
            MediumRatingMax = 1200   // NEW
        };
    }
}
