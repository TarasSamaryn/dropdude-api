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
            public int Damage { get; set; }
        }

        [HttpPost("finish")]
        [Authorize]
        public async Task<IActionResult> Finish([FromBody] RecordResultDto dto)
        {
            _logger.LogInformation("🔔 Finish hit: {@Dto}", dto);

            // 1) Зберігаємо результат бою з уронем
            var result = new GameResult
            {
                PlayerId   = dto.WinnerId,
                OccurredAt = DateTimeOffset.UtcNow,
                Damage     = dto.Damage
            };
            _db.GameResults.Add(result);
            await _db.SaveChangesAsync();

            // 2) Шукаємо гравця
            var player = await _db.Players.FindAsync(dto.WinnerId);
            if (player == null)
            {
                _logger.LogWarning("⚠️ Player not found: {Id}", dto.WinnerId);
                return BadRequest("Гравець не знайдений");
            }

            // 3) Збільшуємо перемоги
            player.MonthlyWins++;

            // 4) Розраховуємо рейтинг — середній урон за останні 20 боїв (включно з поточним)
            var last20 = await _db.GameResults
                .Where(r => r.PlayerId == dto.WinnerId)
                .OrderByDescending(r => r.OccurredAt)
                .Take(20)
                .ToListAsync();

            player.Rating = last20.Any() ? last20.Average(r => r.Damage) : 0;

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "✅ Recorded win, updated MonthlyWins and Rating for PlayerId {Id}: Wins={Wins}, Rating={Rating}",
                dto.WinnerId, player.MonthlyWins, player.Rating
            );

            return Ok(new
            {
                message   = "Переможець зафіксовано",
                newRating = player.Rating
            });
        }

        [HttpGet("leaderboard")]
        [Authorize]
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
            _logger.LogInformation("🔔 ResetMonthlyWins hit");

            List<Player> players = await _db.Players.ToListAsync();
            foreach (Player p in players)
            {
                p.MonthlyWins = 0;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation("✅ MonthlyWins reset for all players");
            return Ok("Лічильники скинуто");
        }

        [HttpGet("leaderboard/all")]
        [Authorize]
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
        
        [HttpGet("rating-leaderboard")]
        [Authorize]
        public async Task<IActionResult> GetRatingLeaderboard()
        {
            _logger.LogInformation("🔔 Rating leaderboard hit");
            var list = await _db.Players
                .OrderByDescending(p => p.Rating)
                .Select(p => new { p.Username, p.Rating })
                .ToListAsync();
            return Ok(list);
        }
    }
}
