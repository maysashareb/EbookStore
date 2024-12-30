using EbookStore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EbookStore.Models
{
    public class CartItems
    {
        [Key]
        public int CartItemID { get; set; }

        [Required]
        [ForeignKey("Cart")]
        public int CartID { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }




        public virtual Cart Cart { get; set; }
        public virtual Book Book { get; set; }
    }
}