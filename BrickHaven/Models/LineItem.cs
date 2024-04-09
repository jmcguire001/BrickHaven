using System.ComponentModel.DataAnnotations.Schema;

namespace BrickHaven.Models
{
    public class LineItem
    {
        [ForeignKey ("Order")]
        public int TransactionId { get; set; }
        [ForeignKey ("Product")]
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
        public int? Rating { get; set; }
    }
}
