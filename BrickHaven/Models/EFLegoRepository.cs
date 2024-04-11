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
