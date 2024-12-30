using System.ComponentModel.DataAnnotations.Schema;

namespace EbookStore.Models
{
    public class Cart
    {
        public int CartID { get; set; } // Primary Key
        public string UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }


        public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();

        public Cart()
        {
            CartItems = new List<CartItems>();
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount
        {
            get
            {
                return CartItems?.Sum(item => item.Quantity * item.Price) ?? 0;
            }
        }
    }
}


