using AuthenticationService.Model.Database.Types;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Model.Database
{
    public class AuthenticationContext : DbContext
    {
        public DbSet<User> User { get; set; }

        public AuthenticationContext() { }
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (builder.IsConfigured) return;

            const string connectionString = "User ID=postgres;Password=Test123!;Host=localhost;Port=5432;Database=salamdo_authentication;Pooling=true;Connection Lifetime=0;";

            builder.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
