using CatalogueService.Model.Database.Types;
using Microsoft.EntityFrameworkCore;

namespace CatalogueService.Model
{
    public class CatalogueContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<FilterPropertyDefinition> FilterPropertyDefinitions { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<FilterProperty> FilterProperties { get; set; }

        public CatalogueContext()
        {
            
        }
        public CatalogueContext(DbContextOptions<CatalogueContext> options): base(options)
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "User ID=postgres;" +
                    "Password=Test123!;" +
                    "Host=localhost;" +
                    "Port=5432;" +
                    "Database=shopping-app_catalogue-service;" +
                    "Pooling=true;" +
                    "Connection Lifetime=0;"
                    ).UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
        
    }
}
