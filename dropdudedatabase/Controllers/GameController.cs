using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinefieldServer.Data;
using MinefieldServer.Models;

namespace MinefieldServer.Controllers
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
    }
}
