using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BrickHaven.Models
{
    public class LegoContext : DbContext
    {
        // Constructor. Inherit base options; provide way to enter your own options
        public LegoContext(DbContextOptions<LegoContext> options) : base(options)
        {
        }

        // Create a public set that consists of instances of Lego. Saves individual legos into Sets (aka table called 'Lego')
        public DbSet<Order> Orders { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LineItem>().HasNoKey(); // Configure LineItem as a keyless entity type
        }

    }
}