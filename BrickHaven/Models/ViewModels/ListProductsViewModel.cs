namespace BrickHaven.Models.ViewModels
{
    public class ListProductsViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public int? CurrentPageSize { get; set; }
        public string? Category { get; set; }
    }
}
