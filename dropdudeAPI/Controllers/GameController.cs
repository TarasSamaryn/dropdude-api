using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinefieldServer.Data;
using MinefieldServer.Models;

namespace MinefieldServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<GameController> _logger;

        public GameController(AppDbContext db, ILogger<GameController> logger)
        {
            _db     = db;
            _logger = logger;
        }

        public class RecordResultDto
        {
            public int WinnerId { get; set; }
        }

        [HttpPost("finish")]
        [Authorize]  // раніше RequireServiceToken, тепер всі залогінені
        public async Task<IActionResult> Finish([FromBody] RecordResultDto dto)
        {
            _logger.LogInformation("🔔 Finish hit: {@Dto}", dto);

            var result = new GameResult
            {
                PlayerId   = dto.WinnerId,
                OccurredAt = DateTimeOffset.UtcNow
            };
            _db.GameResults.Add(result);

            var player = await _db.Players.FindAsync(dto.WinnerId);
            if (player == null)
            {
                _logger.LogWarning("⚠️ Player not found: {Id}", dto.WinnerId);
                return BadRequest("Гравець не знайдений");
            }

            player.MonthlyWins++;
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "✅ Recorded win and ++MonthlyWins for PlayerId {Id}",
                dto.WinnerId
            );
            return Ok(new { message = "Переможець зафіксовано" });
        }

        [HttpGet("leaderboard")]
        [Authorize]  // захищено JWT
        public async Task<IActionResult> GetLeaderboard()
        {
            _logger.LogInformation("🔔 Leaderboard hit");

            var monthStart = new DateTimeOffset(
                DateTime.UtcNow.Year,
                DateTime.UtcNow.Month,
                1, 0, 0, 0,
                TimeSpan.Zero
            );

            var top = await _db.GameResults
                .Where(r => r.OccurredAt >= monthStart)
                .GroupBy(r => r.PlayerId)
                .Select(g => new { PlayerId = g.Key, Wins = g.Count() })
                .OrderByDescending(x => x.Wins)
                .FirstOrDefaultAsync();

            if (top == null)
            {
                _logger.LogWarning("⚠️ No monthly results");
                return NotFound("Немає результатів за місяць");
            }

            _logger.LogInformation(
                "✅ Leaderboard: PlayerId {Id} → {Wins} wins",
                top.PlayerId, top.Wins
            );
            return Ok(top);
        }

        [HttpGet("champion")]
        public async Task<IActionResult> GetMonthlyChampion()
        {
            _logger.LogInformation("🔔 Champion hit");

            var champ = await _db.Players
                .OrderByDescending(p => p.MonthlyWins)
                .FirstOrDefaultAsync();

            if (champ == null)
            {
                _logger.LogWarning("⚠️ No players found");
                return NotFound("Немає гравців");
            }

            return Ok(new
            {
                champ.Id,
                champ.Username,
                champ.MonthlyWins
            });
        }

        [HttpPost("reset-monthly")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> ResetMonthlyWins()
        {
            return Ok("Лічильники скинуто");
            
            _logger.LogInformation("🔔 ResetMonthlyWins hit");

            var players = await _db.Players.ToListAsync();
            players.ForEach(p => p.MonthlyWins = 0);
            await _db.SaveChangesAsync();

            _logger.LogInformation("✅ MonthlyWins reset for all players");
            return Ok("Лічильники скинуто");
        }
        
        [HttpGet("leaderboard/all")]
        [Authorize]  // захищено JWT
        public async Task<IActionResult> GetFullLeaderboard()
        {
            var monthStart = new DateTimeOffset(
                DateTime.UtcNow.Year,
                DateTime.UtcNow.Month,
                1, 0, 0, 0,
                TimeSpan.Zero
            );

            var list = await _db.GameResults
                .Where(r => r.OccurredAt >= monthStart)
                .GroupBy(r => r.PlayerId)
                .Select(g => new { PlayerId = g.Key, Wins = g.Count() })
                .OrderByDescending(x => x.Wins)
                .Join(_db.Players,
                      g => g.PlayerId,
                      p => p.Id,
                      (g, p) => new { p.Username, g.Wins })
                .ToListAsync();

            return Ok(list);
        }
    }
}
