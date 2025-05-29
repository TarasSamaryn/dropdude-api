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

        public ProfileController(AppDbContext db)
        {
            _db = db;
        }

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
            list.Add(skinId);
            player.HeadsSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added head skin {skinId}" });
        }

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
            list.Add(skinId);
            player.BodiesSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added body skin {skinId}" });
        }

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
            list.Add(skinId);
            player.LegsSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added legs skin {skinId}" });
        }

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
            list.Add(skinId);
            player.MasksSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Added mask skin {skinId}" });
        }

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

        [HttpDelete("skins/head/{skinId}")]
        [Authorize]
        public IActionResult RemoveHeadSkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.HeadsSkins.ToList();
            if (!list.Contains(skinId))
            {
                return BadRequest(new { error = "Head skin not found" });
            }

            list.Remove(skinId);
            player.HeadsSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Removed head skin {skinId}" });
        }

        [HttpDelete("skins/body/{skinId}")]
        [Authorize]
        public IActionResult RemoveBodySkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.BodiesSkins.ToList();
            if (!list.Contains(skinId))
            {
                return BadRequest(new { error = "Body skin not found" });
            }

            list.Remove(skinId);
            player.BodiesSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Removed body skin {skinId}" });
        }

        [HttpDelete("skins/legs/{skinId}")]
        [Authorize]
        public IActionResult RemoveLegsSkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.LegsSkins.ToList();
            if (!list.Contains(skinId))
            {
                return BadRequest(new { error = "Legs skin not found" });
            }

            list.Remove(skinId);
            player.LegsSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Removed legs skin {skinId}" });
        }

        [HttpDelete("skins/mask/{skinId}")]
        [Authorize]
        public IActionResult RemoveMaskSkin(int skinId)
        {
            Player player = GetCurrentPlayer();
            if (player == null)
            {
                return Unauthorized();
            }

            List<int> list = player.MasksSkins.ToList();
            if (!list.Contains(skinId))
            {
                return BadRequest(new { error = "Mask skin not found" });
            }

            list.Remove(skinId);
            player.MasksSkins = list.ToArray();
            _db.SaveChanges();

            return Ok(new { message = $"Removed mask skin {skinId}" });
        }
    }

    public class SkinSelectionDto
    {
        public string SelectedSkin { get; set; } = null!;
    }
}