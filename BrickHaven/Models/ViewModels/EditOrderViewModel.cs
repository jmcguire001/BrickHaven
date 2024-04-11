using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models.ViewModels
{
    public class EditOrderViewModel
    {
        public int TransactionId { get; set; }
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime? Date { get; set; }
        [Display(Name = "Weekday")]
        public string? Weekday { get; set; }
        [Display(Name = "Time")]
        public int? Time { get; set; }
        [Display(Name = "Card Type")]
        public string? CardType { get; set; }
        [Display(Name = "Entry Mode")]
        public string? EntryMode { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public float? Amount { get; set; }
        [Display(Name = "Transaction Type")]
        public string? TransactionType { get; set; }
        [Display(Name = "Transaction Country")]
        public string? TransactionCountry { get; set; }
        [Display(Name = "Shipping Address")]
        public string? ShippingAddress { get; set; }
        [Display(Name = "Bank")]
        public string? Bank { get; set; }
        [Display(Name = "Is Fraud?")]
        public bool? IsFraud { get; set; }

        public IEnumerable<Customer> Customers { get; set; }
    }
}
