using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Route("game")]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<GameController> _logger;

        public GameController(AppDbContext db, ILogger<GameController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ==== DTOs ====
        public class RecordResultDto
        {
            public int Damage { get; set; }
            public bool IsWinner { get; set; }
        }

        public class LeaderboardItemDto
        {
            public int PlayerId { get; set; }
            public string Username { get; set; } = string.Empty;
            public int Wins { get; set; }
            public double Rating { get; set; }
        }

        // ==== NEW: кожен гравець шле свій результат ====
        [HttpPost("result")]
        [Authorize]
        public async Task<IActionResult> PostResult([FromBody] RecordResultDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("🔔 Result by user {UserId}: Damage={Damage}, IsWinner={IsWinner}", userId, dto.Damage, dto.IsWinner);

            _db.GameResults.Add(new GameResult
            {
                PlayerId = userId,
                OccurredAt = DateTimeOffset.UtcNow,
                Damage = dto.Damage
            });

            Player? player = await _db.Players.FindAsync(userId);
            if (player == null)
            {
                _logger.LogWarning("⚠️ Player not found: {Id}", userId);
                return BadRequest("Player not found");
            }
            if (dto.IsWinner) player.MonthlyWins++;

            List<GameResult> last20 = await _db.GameResults
                .Where(r => r.PlayerId == userId)
                .OrderByDescending(r => r.OccurredAt)
                .Take(20)
                .ToListAsync();

            player.Rating = last20.Any() ? last20.Average(r => r.Damage) : 0;

            await _db.SaveChangesAsync();

            _logger.LogInformation("✅ Saved result. Player {Id}: Wins={Wins}, Rating={Rating}", userId, player.MonthlyWins, player.Rating);

            return Ok(new
            {
                message = "Result recorded",
                newRating = player.Rating,
                monthlyWins = player.MonthlyWins
            });
        }

        // ==== LEGACY (краще не використовувати) ====
        [HttpPost("finish")]
        [Authorize]
        public async Task<IActionResult> Finish([FromBody] LegacyFinishDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _logger.LogInformation("🔔 Legacy Finish by user {UserId}. Damage={Damage}", userId, dto.Damage);

            _db.GameResults.Add(new GameResult
            {
                PlayerId = userId,
                OccurredAt = DateTimeOffset.UtcNow,
                Damage = dto.Damage
            });

            Player? player = await _db.Players.FindAsync(userId);
            if (player == null)
            {
                _logger.LogWarning("⚠️ Player not found: {Id}", userId);
                return BadRequest("Player not found");
            }

            player.MonthlyWins++;

            List<GameResult> last20 = await _db.GameResults
                .Where(r => r.PlayerId == userId)
                .OrderByDescending(r => r.OccurredAt)
                .Take(20)
                .ToListAsync();

            player.Rating = last20.Any() ? last20.Average(r => r.Damage) : 0;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Winner recorded (legacy)",
                newRating = player.Rating,
                monthlyWins = player.MonthlyWins
            });
        }

        public class LegacyFinishDto
        {
            public int Damage { get; set; }
            public int WinnerId { get; set; } // ігнорується
        }

        // ==== Лідерборд МІСЯЦЯ: ТЕПЕР СПИСОК ====
        [HttpGet("leaderboard")]
        [Authorize]
        public async Task<IActionResult> GetLeaderboard()
        {
            _logger.LogInformation("🔔 Leaderboard (list) hit");
            var list = await _db.Players
                .OrderByDescending(p => p.MonthlyWins)
                .ThenByDescending(p => p.Rating)
                .Select(p => new LeaderboardItemDto
                {
                    PlayerId = p.Id,
                    Username = p.Username,
                    Wins = p.MonthlyWins,
                    Rating = p.Rating
                })
                .ToListAsync();

            return Ok(list);
        }

        // Повний список (залишаю для сумісності; еквівалентно /game/leaderboard)
        [HttpGet("leaderboard/all")]
        [Authorize]
        public async Task<IActionResult> GetFullLeaderboard()
        {
            var list = await _db.Players
                .OrderByDescending(p => p.MonthlyWins)
                .ThenByDescending(p => p.Rating)
                .Select(p => new LeaderboardItemDto
                {
                    PlayerId = p.Id,
                    Username = p.Username,
                    Wins = p.MonthlyWins,
                    Rating = p.Rating
                })
                .ToListAsync();

            return Ok(list);
        }

        // Додатково: лідерборд за рейтингом
        [HttpGet("leaderboard/by-rating")]
        [Authorize]
        public async Task<IActionResult> GetByRating()
        {
            var list = await _db.Players
                .OrderByDescending(p => p.Rating)
                .ThenByDescending(p => p.MonthlyWins)
                .Select(p => new LeaderboardItemDto
                {
                    PlayerId = p.Id,
                    Username = p.Username,
                    Wins = p.MonthlyWins,
                    Rating = p.Rating
                })
                .ToListAsync();

            return Ok(list);
        }

        // Чемпіон місяця (сумісність зі старим клієнтом)
        [HttpGet("champion")]
        public async Task<IActionResult> GetMonthlyChampion()
        {
            _logger.LogInformation("🔔 Champion hit");
            Player? champ = await _db.Players.OrderByDescending(p => p.MonthlyWins).FirstOrDefaultAsync();

            if (champ == null) return NotFound("No players");

            return Ok(new
            {
                champ.Id,
                champ.Username,
                champ.MonthlyWins
            });
        }

        // Скидання лічильника перемог (адмін)
        [HttpPost("reset-monthly")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> ResetMonthlyWins()
        {
            _logger.LogInformation("🔔 ResetMonthlyWins hit");

            List<Player> players = await _db.Players.ToListAsync();
            foreach (Player p in players) p.MonthlyWins = 0;

            await _db.SaveChangesAsync();
            _logger.LogInformation("✅ MonthlyWins reset for all players");

            return Ok("Counters reset");
        }
    }
}
