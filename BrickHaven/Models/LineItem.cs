using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrickHaven.Models
{
    public class LineItem
    {
        [Key]
        public int LineItemId { get; set; }
        [ForeignKey("Order")]
        public int? TransactionId { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public int? Rating { get; set; }
    }
}