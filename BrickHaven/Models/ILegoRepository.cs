using Microsoft.EntityFrameworkCore;

namespace BrickHaven.Models
{
    public interface ILegoRepository
    {
        // Query the instances from Lego model and save to set Lego
        public IQueryable<Product> Products { get; }
        public IQueryable<Order> Orders { get; }

        // Update the product
        Task<int> SaveChangesAsync();
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task AddProduct (Product product);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task AddOrder(Order order);

        public Product GetProductById(int id);
        //public IQueryable<Customer> Customers { get; }
        public void AddToCart(Product product);
        public void DeleteTask(Task task); // Method for deleting tasks from the views (This is for when the user deletes)
    }
}
