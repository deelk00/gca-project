using Microsoft.EntityFrameworkCore;

namespace CartService.Database
{
    public class CartContext : DbContext
    {


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
