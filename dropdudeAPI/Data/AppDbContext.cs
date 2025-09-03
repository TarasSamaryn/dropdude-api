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
        public DbSet<ServerGameSettings> ServerGameSettings { get; set; } = null!; // ✅ нова таблиця

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // GameSettings: зберігаємо масиви як integer[]
            modelBuilder.Entity<GameSettings>(entity =>
            {
                entity.Property(e => e.FreeSkins).HasColumnType("integer[]");
                entity.Property(e => e.MonthlySkins).HasColumnType("integer[]");
            });

            // ServerGameSettings: також зберігаємо масив як integer[]
            modelBuilder.Entity<ServerGameSettings>(entity =>
            {
                entity.Property(e => e.FreeSkins).HasColumnType("integer[]");
            });
        }
    }
}