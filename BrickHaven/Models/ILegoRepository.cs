using Microsoft.EntityFrameworkCore;

namespace BrickHaven.Models
{
    public interface ILegoRepository
    {
        // Query the instances from Lego model and save to set Lego
        public IQueryable<Product> Products { get; }
        public IQueryable<Order> Orders { get; }
        public IQueryable<LineItem> LineItems { get; }

        // Update the product
        Task<int> SaveChangesAsync();
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task AddProduct(Product product);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
        Task AddLineItem(LineItem lineItem);
        Task AddOrder(Order order);

        public Product GetProductById(int id);
        //public IQueryable<Customer> Customers { get; }
        public void AddToCart(Product product);
    }
}