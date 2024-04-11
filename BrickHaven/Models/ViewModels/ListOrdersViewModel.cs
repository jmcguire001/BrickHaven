namespace BrickHaven.Models.ViewModels
{
    public class ListOrdersViewModel
    {
        public IQueryable<Product> Products { get; set; }
        public IQueryable<Order> Orders { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public int? CurrentPageSize { get; set; }
    }
}
