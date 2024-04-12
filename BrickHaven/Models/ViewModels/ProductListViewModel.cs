namespace BrickHaven.Models.ViewModels
{
    public class ProductListViewModel
    {
        public IQueryable<Product> Products { get; set; } // This is a collection of <Lego> instances
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo(); // WHY DO WE NOT NEED TO <> THIS? We don't need <> because PaginationInfo is just a class, not a collection
        public string? CurrentLegoType { get; set; } // We can set the current lego type
        public string? CurrentLegoCategory { get; set; } // We can set the current category
        public string? CurrentLegoColor { get; set; } // We can set the current lego color

        public int? CurrentPageSize { get; set; }
    }
}