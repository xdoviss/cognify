using Microsoft.EntityFrameworkCore;
using cognify.Shared; 

namespace cognify.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameResult> GameResults { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WordRecallStatistics> WordRecallStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameResult>().HasKey(gr => gr.Id); 
        }
    }
}
