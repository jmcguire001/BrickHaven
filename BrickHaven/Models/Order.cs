using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;

namespace BrickHaven.Models
{
    public class Order
    {
        [Key]
        public int TransactionId { get; set; }
        public int? CustomerId { get; set; }

        //[ForeignKey("Customer")]
        public string? UserId { get; set; }
        public DateTime? Date { get; set; }
        public string? Weekday { get; set; }
        public int? Time { get; set; }
        public string? CardType { get; set; }
        public string? EntryMode { get; set; }
        public float? Amount { get; set; }
        public string? TransactionType { get; set; }
        public string? TransactionCountry { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Bank { get; set; }
        public bool? IsFraud { get; set; } = false;
    }
}
