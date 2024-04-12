namespace BrickHaven.Models.ViewModels
{
    public class ListUsersViewModel
    {
        public IQueryable<Customer> Customers { get; set; }
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public int? CurrentPageSize { get; set; }
        public string? Role { get; set; }
    }
}
