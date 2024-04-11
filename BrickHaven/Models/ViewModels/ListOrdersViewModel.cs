namespace BrickHaven.Models.ViewModels
{
    public class ListOrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public int? CurrentPageSize { get; set; }
        public string? TransactionType { get; set; }
    }
}
