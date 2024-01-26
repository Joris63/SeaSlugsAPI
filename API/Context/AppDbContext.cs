using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Entities;

namespace SeaSlugAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeaSlug>()
                .HasAlternateKey(c => c.Label);
        }

        public DbSet<SeaSlug> SeaSlugs { get; set; }
        public DbSet<TrainingLog> TrainingLogs { get; set; }
    }
}

