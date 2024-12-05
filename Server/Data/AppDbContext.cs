using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Coordinate> Coordinates { get; set; } = null!;
        public DbSet<EnvironmentalDataEntry> EnvironmentalDataEntries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coordinate>().ToTable("Coordinates");
            modelBuilder.Entity<EnvironmentalDataEntry>().ToTable("EnvironmentalData");

            base.OnModelCreating(modelBuilder);
        }
    }
}