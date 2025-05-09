using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public ProfileController(AppDbContext db) => _db = db;

        private Player? GetCurrentPlayer()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClaim, out var id)) return null;
            return _db.Players.Find(id);
        }

        [HttpGet, Authorize]
        public IActionResult GetProfile()
        {
            var p = GetCurrentPlayer();
            if (p == null) return Unauthorized();
            return Ok(new {
                id = p.Id,
                isAdmin = p.IsAdmin,
                username = p.Username,
                lastSelectedSkin = p.LastSelectedSkin,
                headsSkins = p.HeadsSkins,
                bodiesSkins = p.BodiesSkins,
                legsSkins = p.LegsSkins,
                masksSkins = p.MasksSkins,
            });
        }

        [HttpPost("skins/{type}/{skinId}"), Authorize]
        public IActionResult AddSkin(string type, int skinId)
        {
            var p = GetCurrentPlayer();
            if (p == null) return Unauthorized();

            var list = type.ToLower() switch
            {
                "head"  => p.HeadsSkins.ToList(),
                "body"  => p.BodiesSkins.ToList(),
                "legs"  => p.LegsSkins.ToList(),
                "mask"  => p.MasksSkins.ToList(),
                _       => null
            };
            if (list == null) return BadRequest("Unknown skin type");
            if (list.Contains(skinId)) return BadRequest("Skin already added");

            list.Add(skinId);
            switch (type.ToLower())
            {
                case "head": p.HeadsSkins = list.ToArray(); break;
                case "body": p.BodiesSkins = list.ToArray(); break;
                case "legs": p.LegsSkins = list.ToArray(); break;
                case "mask": p.MasksSkins = list.ToArray(); break;
            }
            _db.SaveChanges();
            return Ok(new { message = $"Added {type} skin {skinId}" });
        }

        [HttpPost("skin/select"), Authorize]
        public IActionResult SelectSkin([FromBody] SkinSelectionDto dto)
        {
            var p = GetCurrentPlayer();
            if (p == null) return Unauthorized();

            bool owned = new[]{
                p.HeadsSkins, p.BodiesSkins, p.LegsSkins, p.MasksSkins
            }.Any(arr => arr.Contains(dto.SkinId));
            if (!owned) return BadRequest("Skin not owned");

            p.LastSelectedSkin = dto.SkinId.ToString();
            _db.SaveChanges();
            return Ok(new { message = $"Selected skin {dto.SkinId}" });
        }
    }

    public class SkinSelectionDto
    {
        public int SkinId { get; set; }
    }
}
