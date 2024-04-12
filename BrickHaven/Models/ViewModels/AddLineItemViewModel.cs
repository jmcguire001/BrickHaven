namespace BrickHaven.Models.ViewModels
{
    public class AddLineItemViewModel
    {
        public int? LineItemId { get; set; }
        public int? TransactionId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }
}
