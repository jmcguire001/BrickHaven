using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace BrickHaven.Models
{
    public class Cart
    {
        public class CartLine
        {
            public int CartLineId { get; set; }
            public Product Product { get; set; }
            public int Quantity { get; set; }
            public float Price { get; set; }
            public float Amount { get; set; }
        }

        public List<CartLine> Lines { get; set; } = new List<CartLine>(); // Equals sign means if we haven't built a list, then build one

        public void AddItem(Product p, int quantity)
        {
            // Add the item to the cart first
            CartLine? line = Lines
                .Where(x => x.Product.ProductId == p.ProductId)
                .FirstOrDefault(); // Makes sure you only get one

            // Has this item already been added to the cart?
            if (line == null) // Add a new item if line is null
            {
                Lines.Add(new CartLine
                {
                    Product = p,
                    Quantity = quantity,
                    Price = (float)p.Price,
                    Amount = (float)p.Price * quantity
                });
            }
            else
            {
                line.Quantity += quantity; // Update the count
            }
        }

        public void RemoveLine(Product p) => Lines.RemoveAll(x => x.Product.ProductId == p.ProductId);

        public void Clear() => Lines.Clear();

        public float CalculateTotal() => Lines.Sum(x => x.Price * x.Quantity); // Use Lambda function to get total
    }
}