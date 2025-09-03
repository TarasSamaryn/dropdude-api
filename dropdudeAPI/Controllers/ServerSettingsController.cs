using DropDudeAPI.Data;
using DropDudeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DropDudeAPI.Controllers
{
    [ApiController]
    [Route("server-settings")]
    public class ServerSettingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ServerSettingsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /server-settings
        [HttpGet]
        public async Task<IActionResult> GetServerSettings()
        {
            var settings = await _db.ServerGameSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new ServerGameSettings
                {
                    FreeSkins = new[] { 0, 6, 7, 41}
                };
                _db.ServerGameSettings.Add(settings);
                await _db.SaveChangesAsync();
            }
            return Ok(settings);
        }

        // PUT /server-settings (Admin only)
        [HttpPut]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> UpdateServerSettings([FromBody] ServerGameSettings input)
        {
            var settings = await _db.ServerGameSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                return NotFound(new { error = "Server settings not found" });
            }

            settings.FreeSkins = input.FreeSkins;
            await _db.SaveChangesAsync();

            return Ok(settings);
        }
    }
}