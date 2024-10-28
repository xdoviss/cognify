using Microsoft.EntityFrameworkCore;
using cognify.Shared; 

namespace cognify.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameResult> GameResults { get; set; }
        public DbSet<WordRecallStatistics> WordRecallStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure GameResult entity (if Id is present as a primary key)
            modelBuilder.Entity<GameResult>().HasKey(gr => gr.Id); // Ensure GameResult has an Id property

            // Add configurations for additional properties if needed
            // Example: modelBuilder.Entity<GameType>().Property(gt => gt.SomeProperty).IsRequired();
        }
    }
}
