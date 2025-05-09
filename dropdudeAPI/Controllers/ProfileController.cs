using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MinefieldServer.Data;
using MinefieldServer.Models;

namespace MinefieldServer.Controllers
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

        // Допоміжний метод для отримання поточного гравця за токеном
        private Player GetCurrentPlayer()
        {
            string idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool parsed = Int32.TryParse(idClaim, out int id);
            if (parsed == false)
            {
                return null;
            }

            Player player = _db.Players.Find(id);
            return player;
        }

        // 1. Отримати профіль
        [HttpGet]
        [Authorize]
        public IActionResult GetProfile()
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized(new { error = "Invalid token or player not found" });
            }

            return Ok(new
            {
                id = player.Id,
                isAdmin = player.IsAdmin,
                username = player.Username,
                lastSelectedSkin = player.LastSelectedSkin,
                headsSkins = player.HeadsSkins,
                bodiesSkins = player.BodiesSkins,
                legsSkins = player.LegsSkins,
                masksSkins = player.MasksSkins
            });
        }

        // 2. Додати скін голови
        [HttpPost("skins/head/{skinId}")]
        [Authorize]
        public IActionResult AddHeadSkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.HeadsSkins.ToList();
            if (list.Contains(skinId))
            {
                return BadRequest(new { error = "Head skin already added" });
            }

            list.Add(skinId);
            player.HeadsSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added head skin {skinId}" });
        }

        // 3. Додати скін тіла
        [HttpPost("skins/body/{skinId}")]
        [Authorize]
        public IActionResult AddBodySkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.BodiesSkins.ToList();
            if (list.Contains(skinId))
            {
                return BadRequest(new { error = "Body skin already added" });
            }

            list.Add(skinId);
            player.BodiesSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added body skin {skinId}" });
        }

        // 4. Додати скін ніг
        [HttpPost("skins/legs/{skinId}")]
        [Authorize]
        public IActionResult AddLegsSkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.LegsSkins.ToList();
            if (list.Contains(skinId))
            {
                return BadRequest(new { error = "Legs skin already added" });
            }

            list.Add(skinId);
            player.LegsSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added legs skin {skinId}" });
        }

        // 5. Додати скін маски
        [HttpPost("skins/mask/{skinId}")]
        [Authorize]
        public IActionResult AddMaskSkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.MasksSkins.ToList();
            if (list.Contains(skinId))
            {
                return BadRequest(new { error = "Mask skin already added" });
            }

            list.Add(skinId);
            player.MasksSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added mask skin {skinId}" });
        }

        // 6. Перезаписати LastSelectedSkin у форматі string
        [HttpPost("skin/select")]
        [Authorize]
        public IActionResult SelectSkin([FromBody] SkinSelectionDto dto)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized(new { error = "Invalid token or player not found" });
            }

            player.LastSelectedSkin = dto.SelectedSkin;
            _db.SaveChanges();

            return Ok(new { message = $"Selected skin {dto.SelectedSkin}" });
        }
    }

    // DTO для вибору скіну у вигляді рядка
    public class SkinSelectionDto
    {
        public string SelectedSkin { get; set; }
    }
}
