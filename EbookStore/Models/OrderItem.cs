using EbookStore.Models;

namespace EbookStore.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; } // Primary Key
        public int OrderID { get; set; } // Foreign Key to Orders
        public int BookID { get; set; } // Foreign Key to Books table
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price of the book at the time of purchase

        // Navigation Properties
        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}
