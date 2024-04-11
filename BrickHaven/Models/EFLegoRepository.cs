using Microsoft.EntityFrameworkCore;
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

        public IQueryable<Order> Orders => _context.Orders;

        public Product GetProductById(int productId)

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}
