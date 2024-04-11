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
   
        public Product GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == productId);
        }
        public void AddToCart(Product product) // Method is responsible for adding a new task to the database
        {
            _context.Add(product);
            _context.SaveChanges();
        }

        public void UpdateTask(Task task) // Method is responsible for updating a tasks to the database
        {
            _context.Update(task);
            _context.SaveChanges();
        }

        public void DeleteTask(Task task) // Method is responsible for removing tasks to from the database
        {
            _context.Remove(task);
            _context.SaveChanges();
        }
    }
}
