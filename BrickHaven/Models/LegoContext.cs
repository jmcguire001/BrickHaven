using Microsoft.EntityFrameworkCore;

namespace BrickHaven.Models
{
    public class LegoContext : DbContext
    {
        // Constructor. Inherit base options; provide way to enter your own options
        public LegoContext(DbContextOptions<LegoContext> options) : base(options)
        {
        }

        // Create a public set that consists of instances of Lego. Saves individual legos into Sets (aka table called 'Lego')
        public DbSet<Lego> Legos { get; set; }
    }
}
