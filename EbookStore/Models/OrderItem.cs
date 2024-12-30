using EbookStore.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderItem
{
    public int OrderItemID { get; set; } // Primary Key

    public int OrderID { get; set; } // Foreign Key to Orders
    public int BookId { get; set; } // Foreign Key to Books
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

   
    // Navigation Properties
    public virtual Order Order { get; set; } // Reference to Order
    public virtual Book Book { get; set; } // Reference to Book
}
