using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models.ViewModels
{
    public class AddOrderViewModel
    {
        public int? TransactionId { get; set; }
        [Display(Name = "Your User ID")]
        public string? UserId { get; set; }

        [Display(Name = "Today's Date")]
        public DateTime? Date { get; set; }

        [Display(Name = "Today's Day of Week")]
        public string? Weekday { get; set; }

        [Display(Name = "Time of Transaction (24-hr")]
        public int? Time { get; set; }

        [Required]
        [Display(Name = "Card Type")]
        public string? CardType { get; set; }

        [Required]
        [Display(Name = "Entry Mode")]
        public string? EntryMode { get; set; }
        [Display(Name = "Total Amount")]
        public float? Amount { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public string? TransactionType { get; set; }

        [Display(Name = "Country of Transaction")]
        [Required]
        public string? TransactionCountry { get; set; }

        [Required]
        [Display(Name = "Billing Address")]
        public string? BillingAddress { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public string? Bank { get; set; }
    }
}
