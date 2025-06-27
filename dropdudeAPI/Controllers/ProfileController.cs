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

        private Dictionary<int, int> ParseSkins(string skinsString)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            
            if (string.IsNullOrEmpty(skinsString))
            {
                return dict;
            }

            string[] parts = skinsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string part in parts)
            {
                if (part.Contains('-'))
                {
                    string[] tokens = part.Split('-', 2);
                    
                    if (int.TryParse(tokens[0], out int id) && int.TryParse(tokens[1], out int count))
                    {
                        dict[id] = count;
                    }
                }
                else
                {
                    if (int.TryParse(part, out int id))
                    {
                        if (dict.ContainsKey(id))
                        {
                            dict[id]++;
                        }
                        else
                        {
                            dict[id] = 1;
                        }
                    }
                }
            }

            return dict;
        }

        private string SerializeSkins(Dictionary<int, int> dict)
        {
            List<string> parts = new List<string>();
            
            foreach (KeyValuePair<int, int> kv in dict)
            {
                if (kv.Value <= 1)
                {
                    parts.Add(kv.Key.ToString());
                }
                else
                {
                    parts.Add($"{kv.Key}-{kv.Value}");
                }
            }

            return string.Join(",", parts);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetProfile()
        {
            Player player = GetCurrentPlayer();
            
            if (player == null)
            {
                return Unauthorized(new
                {
                    error = "Invalid token or player not found"
                });
            }

            Dictionary<int, int> headsDict  = ParseSkins(player.HeadsSkins);
            Dictionary<int, int> bodiesDict = ParseSkins(player.BodiesSkins);
            Dictionary<int, int> legsDict   = ParseSkins(player.LegsSkins);
            Dictionary<int, int> masksDict  = ParseSkins(player.MasksSkins);

            return Ok(new
            {
                id = player.Id,
                isAdmin = player.IsAdmin,
                username = player.Username,
                lastSelectedSkin = player.LastSelectedSkin,
                headsSkins = headsDict.SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value)).ToArray(),
                bodiesSkins = bodiesDict.SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value)).ToArray(),
                legsSkins = legsDict.SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value)).ToArray(),
                masksSkins = masksDict.SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value)).ToArray(),
                rating = player.Rating,
                monthlyWins = player.MonthlyWins,
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

            Dictionary<int, int> dict = ParseSkins(player.HeadsSkins);
            
            if (dict.ContainsKey(skinId))
            {
                dict[skinId]++;
            }
            else
            {
                dict[skinId] = 1;
            }

            player.HeadsSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Added head skin {skinId}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.BodiesSkins);
            
            if (dict.ContainsKey(skinId))
            {
                dict[skinId]++;
            }
            else
            {
                dict[skinId] = 1;
            }

            player.BodiesSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Added body skin {skinId}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.LegsSkins);
            
            if (dict.ContainsKey(skinId))
            {
                dict[skinId]++;
            }
            else
            {
                dict[skinId] = 1;
            }

            player.LegsSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Added legs skin {skinId}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.MasksSkins);
            
            if (dict.ContainsKey(skinId))
            {
                dict[skinId]++;
            }
            else
            {
                dict[skinId] = 1;
            }

            player.MasksSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Added mask skin {skinId}"
            });
        }

        [HttpPost("skin/select")]
        [Authorize]
        public IActionResult SelectSkin([FromBody] SkinSelectionDto dto)
        {
            Player player = GetCurrentPlayer();
            
            if (player == null)
            {
                return Unauthorized(new
                {
                    error = "Invalid token or player not found"
                });
            }

            player.LastSelectedSkin = dto.SelectedSkin;
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Selected skin {dto.SelectedSkin}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.HeadsSkins);
            
            if (!dict.ContainsKey(skinId))
            {
                return BadRequest(new
                {
                    error = "Head skin not found"
                });
            }

            if (dict[skinId] > 1)
            {
                dict[skinId]--;
            }
            else
            {
                dict.Remove(skinId);
            }

            player.HeadsSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Removed head skin {skinId}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.BodiesSkins);
            
            if (!dict.ContainsKey(skinId))
            {
                return BadRequest(new
                {
                    error = "Body skin not found"
                });
            }

            if (dict[skinId] > 1)
            {
                dict[skinId]--;
            }
            else
            {
                dict.Remove(skinId);
            }

            player.BodiesSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Removed body skin {skinId}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.LegsSkins);
            
            if (!dict.ContainsKey(skinId))
            {
                return BadRequest(new
                {
                    error = "Legs skin not found"
                });
            }

            if (dict[skinId] > 1)
            {
                dict[skinId]--;
            }
            else
            {
                dict.Remove(skinId);
            }

            player.LegsSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Removed legs skin {skinId}"
            });
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

            Dictionary<int, int> dict = ParseSkins(player.MasksSkins);
            
            if (!dict.ContainsKey(skinId))
            {
                return BadRequest(new
                {
                    error = "Mask skin not found"
                });
            }

            if (dict[skinId] > 1)
            {
                dict[skinId]--;
            }
            else
            {
                dict.Remove(skinId);
            }

            player.MasksSkins = SerializeSkins(dict);
            _db.SaveChanges();

            return Ok(new { message = $"Removed mask skin {skinId}" });
        }
    }

    public class SkinSelectionDto
    {
        public string SelectedSkin { get; set; } = null!;
    }
}
