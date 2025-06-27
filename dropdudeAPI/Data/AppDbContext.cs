using DropDudeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DropDudeAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Player> Players => Set<Player>();
    public DbSet<GameResult> GameResults { get; set; } = null!;
}