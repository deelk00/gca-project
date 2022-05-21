using AuthService.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthService.Database
{
    public class AuthenticationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Privilege> Privileges { get; set; }

        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseNpgsql("User ID=postgres;Password=Test123!;Host=localhost;Port=5432;Database=webshop_authentication;Pooling=true;Connection Lifetime=0;")
                    .UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>(user =>
            {
                user.HasMany(x => x.Roles)
                    .WithMany(x => x.Users);

                user.HasKey(x => x.Id);
                
                user.Property(x => x.Email)
                    .HasMaxLength(320)
                    .IsRequired();

                user.Property(x => x.Password)
                    .HasMaxLength(512)
                    .IsRequired();

                user.Property(x => x.Salt)
                    .HasMaxLength(512)
                    .IsRequired();
            });

            b.Entity<Role>(role =>
            {
                role.HasMany<Privilege>(x => x.Privileges)
                    .WithMany(x => x.Roles);

                role.HasKey(x => x.Id);

                role.Property(x => x.Name)
                    .HasMaxLength(128)
                    .IsRequired();

                role.Property(x => x.Description)
                    .HasMaxLength(256);
            });

            b.Entity<Privilege>(privilege =>
            {
                privilege.HasKey(x => x.Id);

                privilege.Property(x => x.Name)
                    .HasMaxLength(128)
                    .IsRequired();

                privilege.Property(x => x.Description)
                    .HasMaxLength(256);

            });
        }
    }
}
