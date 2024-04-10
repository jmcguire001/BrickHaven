using System.Runtime.CompilerServices;

namespace BrickHaven.Models
{
    public class EFLegoRepository : ILegoRepository
    {
        private LegoContext _context;

        public EFLegoRepository(LegoContext temp)
        {
            _context = temp;
        }

        // Queries from the context file, but is an additional layer
        public IQueryable<Product> Products => _context.Products;

    }
}
