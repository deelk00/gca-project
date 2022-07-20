using CheckoutService.Model.Database.Types;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Model.Database
{
    public class CheckoutContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public CheckoutContext() { }
        public CheckoutContext(DbContextOptions<CheckoutContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (builder.IsConfigured) return;

            const string connectionString = "User ID=postgres;Password=Test123!;Host=localhost;Port=5432;Database=salamdo_checkout-service;Pooling=true;Connection Lifetime=0;";

            builder.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
