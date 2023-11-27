using Microsoft.EntityFrameworkCore;
using SeaSlugAPI.Entities;

namespace SeaSlugAPI.Helpers
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<SeaSlug> SeaSlugs { get; set; }
    }
}

