using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookStore.Models
{
    public class Order
    {
        public int OrderID { get; set; } // Primary Key

        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; } = "Pending"; // Default to "Pending"
        public string PaymentMethod { get; set; } // e.g., "PayPal", "CreditCard"

        [StringLength(450)] // Match AspNetUsers.Id length
        public string UserID { get; set; } // Foreign Key to AspNetUsers

        // Navigation Properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
