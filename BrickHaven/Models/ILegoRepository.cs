using Microsoft.EntityFrameworkCore;

namespace BrickHaven.Models
{
    public interface ILegoRepository
    {
        // Query the instances from Lego model and save to set Lego
        public IQueryable<Product> Products { get; }
        public Product GetProductById(int id);

        public void AddToCart(Product product); 
        public void UpdateTask(Task task); 
        public void DeleteTask(Task task); // Method for deleting tasks from the views (This is for when the user deletes)
    }
}
