using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PlayNexus.Models {
    public class PlayNexusDbContext : IdentityDbContext<User> {
        public PlayNexusDbContext(DbContextOptions<PlayNexusDbContext> options) : base(options) { }

        public DbSet<Highlights> Contents { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Forums> Forums { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<UserSignInLog> UserSignInLogs { get; set; } // Add DbSet for UserSignInLog

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Configure UserSignInLog relationships
            modelBuilder.Entity<UserSignInLog>()
                .HasOne(log => log.User)
                .WithMany()
                .HasForeignKey(log => log.UserId);
        }

        public List<UserSignInLog> GetAllUserSignInLogs()
        {
            return UserSignInLogs.ToList();
        }
    }
}