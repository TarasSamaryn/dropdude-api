using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DropDudeAPI.Data;
using DropDudeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DropDudeAPI.Controllers
{
    [ApiController]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProfileController(AppDbContext db)
        {
            _db = db;
        }

        // Асинхронний хелпер для отримання поточного гравця
        private async Task<Player?> GetAuthorizedPlayerAsync()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClaim, out var userId))
                return null;

            return await _db.Players.FindAsync(userId);
        }

        // Розбір рядка скінів у словник { skinId: count }
        private Dictionary<int, int> ParseSkins(string skinsString)
        {
            var dict = new Dictionary<int, int>();
            if (string.IsNullOrWhiteSpace(skinsString))
                return dict;

            var parts = skinsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.Contains('-'))
                {
                    var tokens = part.Split('-', 2);
                    if (int.TryParse(tokens[0], out var id) &&
                        int.TryParse(tokens[1], out var count))
                    {
                        dict[id] = count;
                    }
                }
                else if (int.TryParse(part, out var id))
                {
                    dict[id] = dict.GetValueOrDefault(id) + 1;
                }
            }

            return dict;
        }

        // Збирання словника скінів назад у рядок
        private string SerializeSkins(Dictionary<int, int> dict)
        {
            var parts = new List<string>();
            foreach (var kv in dict)
            {
                parts.Add(kv.Value > 1
                    ? $"{kv.Key}-{kv.Value}"
                    : kv.Key.ToString());
            }
            return string.Join(",", parts);
        }

        // GET /profile
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var player = await GetAuthorizedPlayerAsync();
            if (player == null)
                return Unauthorized(new { error = "Invalid token or player not found" });

            var skinDict = ParseSkins(player.BodiesSkins);
            var bodiesSkins = skinDict
                .SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value))
                .ToArray();

            // ✅ Безпечне значення для lastSelectedSkin у відповіді (НЕ змінюємо БД)
            string lastSelectedSafe =
                !string.IsNullOrWhiteSpace(player.LastSelectedSkin)
                    ? player.LastSelectedSkin
                    : (bodiesSkins.Length > 0 ? bodiesSkins[0].ToString() : "0");

            var dto = new ProfileDto(
                Id: player.Id,
                IsAdmin: player.IsAdmin,
                Username: player.Username,
                LastSelectedSkin: lastSelectedSafe,
                BodiesSkins: bodiesSkins,
                Rating: player.Rating,
                MonthlyWins: player.MonthlyWins
            );

            return Ok(dto);
        }

        // POST /profile/skins/body/{skinId}
        [HttpPost("skins/body/{skinId}")]
        [Authorize]
        public Task<IActionResult> AddBodySkin(int skinId)
            => ModifySkins(
                getRaw: p => p.BodiesSkins,
                setRaw: (p, raw) => p.BodiesSkins = raw,
                skinId: skinId,
                delta: +1,
                successMessage: $"Added body skin {skinId}"
            );

        // DELETE /profile/skins/body/{skinId}
        [HttpDelete("skins/body/{skinId}")]
        [Authorize]
        public Task<IActionResult> RemoveBodySkin(int skinId)
            => ModifySkins(
                getRaw: p => p.BodiesSkins,
                setRaw: (p, raw) => p.BodiesSkins = raw,
                skinId: skinId,
                delta: -1,
                successMessage: $"Removed body skin {skinId}"
            );

        // POST /profile/skin/select
        [HttpPost("skin/select")]
        [Authorize]
        public async Task<IActionResult> SelectSkin([FromBody] SkinSelectionDto dto)
        {
            var player = await GetAuthorizedPlayerAsync();
            if (player == null)
                return Unauthorized(new { error = "Invalid token or player not found" });

            player.LastSelectedSkin = dto.SelectedSkin;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Selected skin {dto.SelectedSkin}" });
        }

        // Узагальнений метод для додавання/видалення скінів
        private async Task<IActionResult> ModifySkins(
            Func<Player, string> getRaw,
            Action<Player, string> setRaw,
            int skinId,
            int delta,
            string successMessage)
        {
            var player = await GetAuthorizedPlayerAsync();
            if (player == null)
                return Unauthorized();

            var dict = ParseSkins(getRaw(player));

            if (!dict.ContainsKey(skinId) && delta < 0)
                return NotFound(new { error = "Skin not found" });

            dict[skinId] = dict.GetValueOrDefault(skinId) + delta;
            if (dict[skinId] <= 0)
                dict.Remove(skinId);

            setRaw(player, SerializeSkins(dict));
            await _db.SaveChangesAsync();

            return Ok(new { message = successMessage });
        }
    }

    // DTO для відповіді GetProfile
    public record ProfileDto(
        int Id,
        bool IsAdmin,
        string Username,
        string LastSelectedSkin,
        int[] BodiesSkins,
        double Rating,
        int MonthlyWins
    );

    // DTO для вибору скіна
    public class SkinSelectionDto
    {
        public string SelectedSkin { get; set; } = string.Empty;
    }
}
