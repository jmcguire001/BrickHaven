using Microsoft.EntityFrameworkCore;

namespace BrickHaven.Models
{
    public interface ILegoRepository
    {
        // Query the instances from Lego model and save to set Lego
        public IQueryable<Product> Products { get; }

        // Update the product
        Task<int> SaveChangesAsync();
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task AddProduct (Product product);

        public Product GetProductById(int id);

        public void AddToCart(Product product); 
        public void UpdateTask(Task task); 
        public void DeleteTask(Task task); // Method for deleting tasks from the views (This is for when the user deletes)
    }
}
