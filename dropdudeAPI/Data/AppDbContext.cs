using DropDudeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DropDudeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<GameResult> GameResults { get; set; } = null!;
        public DbSet<GameSettings> GameSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL maps C# arrays to SQL arrays automatically,
            // but we’ll explicitly set the column type for clarity:
            modelBuilder.Entity<GameSettings>(entity =>
            {
                entity.Property(e => e.FreeSkins).HasColumnType("integer[]");
                entity.Property(e => e.MonthlySkins).HasColumnType("integer[]");
            });
        }
    }
}