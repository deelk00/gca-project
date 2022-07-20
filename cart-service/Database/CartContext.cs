using CartService.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace CartService.Database
{
    public class CartContext : DbContext
    {
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
        public CartContext() { }
        public CartContext(DbContextOptions<CartContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (builder.IsConfigured) return;
            
            const string connectionString = "User ID=postgres;Password=Test123!;Host=localhost;Port=5432;Database=salamdo_cart;Pooling=true;Connection Lifetime=0;";
            
            builder.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }
    }
}
