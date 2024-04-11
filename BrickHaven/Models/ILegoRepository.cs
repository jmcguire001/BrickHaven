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
    }
}
