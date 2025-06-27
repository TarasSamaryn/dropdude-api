using DropDudeAPI.Data;
using DropDudeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DropDudeAPI.Controllers
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
            
            GameResult result = new GameResult
            {
                PlayerId   = dto.WinnerId,
                OccurredAt = DateTimeOffset.UtcNow,
                Damage     = dto.Damage
            };
            
            _db.GameResults.Add(result);
            await _db.SaveChangesAsync();
            
            Player? player = await _db.Players.FindAsync(dto.WinnerId);
            
            if (player == null)
            {
                _logger.LogWarning("⚠️ Player not found: {Id}", dto.WinnerId);
                return BadRequest("Player not found");
            }
            
            player.MonthlyWins++;
            
            List<GameResult> last20 = await _db.GameResults
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
                message   = "Winner is fixed",
                newRating = player.Rating
            });
        }

        [HttpGet("leaderboard")]
        [Authorize]
        public async Task<IActionResult> GetLeaderboard()
        {
            _logger.LogInformation("🔔 Leaderboard hit");

            DateTimeOffset monthStart = new DateTimeOffset(
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
                return NotFound("No results for the month");
            }

            _logger.LogInformation("✅ Leaderboard: PlayerId {Id} → {Wins} wins", top.PlayerId, top.Wins);
            
            return Ok(top);
        }

        [HttpGet("champion")]
        public async Task<IActionResult> GetMonthlyChampion()
        {
            _logger.LogInformation("🔔 Champion hit");

            Player? champ = await _db.Players.OrderByDescending(p => p.MonthlyWins).FirstOrDefaultAsync();

            if (champ == null)
            {
                _logger.LogWarning("⚠️ No players found");
                return NotFound("No players");
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
            return Ok("Counters reset");
        }

        [HttpGet("leaderboard/all")]
        [Authorize]
        public async Task<IActionResult> GetFullLeaderboard()
        {
            DateTimeOffset monthStart = new DateTimeOffset(
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
