using DropDudeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DropDudeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<GameResult> GameResults { get; set; } = null!;

        // ✅ НОВА таблиця для КЛІЄНТСЬКИХ налаштувань
        public DbSet<ClientGameSettings> ClientGameSettings { get; set; } = null!;

        // ✅ Таблиця для СЕРВЕРНИХ налаштувань
        public DbSet<ServerGameSettings> ServerGameSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ServerGameSettings: масиви для скінів
            modelBuilder.Entity<ServerGameSettings>(entity =>
            {
                entity.Property(e => e.FreeSkins).HasColumnType("integer[]");
                entity.Property(e => e.MonthlySkins).HasColumnType("integer[]");
            });

            // ClientGameSettings: тільки примітиви — додатковий маппінг не потрібен
        }
    }
}