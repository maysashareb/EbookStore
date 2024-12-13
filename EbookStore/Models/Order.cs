using System;
using System.Collections.Generic;

namespace EbookStore.Models
{
    public class Order
    {
        public int OrderID { get; set; } // Primary Key
        public string UserID { get; set; } // Foreign Key to AspNetUsers.Id
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } // e.g., "Pending", "Completed"
        public string PaymentMethod { get; set; } // e.g., "PayPal", "Credit Card"

        // Navigation Properties
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
