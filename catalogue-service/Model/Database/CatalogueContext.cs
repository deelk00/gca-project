using CatalogueService.Model.Database.Types;
using Microsoft.EntityFrameworkCore;

namespace CatalogueService.Model.Database
{
    public class CatalogueContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<FilterProperty> FilterProperties { get; set; }
        public DbSet<FilterPropertyDefinition> FilterPropertyDefinitions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<DatabaseImage> DatabaseImages { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        
        public CatalogueContext() { }
        public CatalogueContext(DbContextOptions<CatalogueContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (builder.IsConfigured) return;
            
            const string connectionString = "User ID=postgres;Password=Test123!;Host=localhost;Port=5432;Database=salamdo_catalogue;Pooling=true;Connection Lifetime=0;";
            
            builder.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
