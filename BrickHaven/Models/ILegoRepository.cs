﻿using Microsoft.EntityFrameworkCore;

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
    }
}
