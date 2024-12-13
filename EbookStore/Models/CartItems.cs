using System.ComponentModel.DataAnnotations;

namespace EbookStore.Models
{
    public class CartItems
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }

        [Required]
        public string TransactionType { get; set; }

        public Cart Cart { get; set; }
        public Book Book { get; set; }
    }
}
