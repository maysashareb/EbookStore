using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookStore.Models
{
    public class UserLibrary
    {
        public int ID { get; set; } // Primary Key

        [StringLength(450)] // Match AspNetUsers.Id length
        public string UserID { get; set; } // Foreign Key to AspNetUsers

        public int BookID { get; set; } // Foreign Key to Books
        public DateTime PurchaseDate { get; set; }
        public int OrderID { get; set; } // Foreign Key to Orders

        [Required]
        [StringLength(50)]
        public string Format { get; set; } // e.g., PDF, EPUB, MOBI, HTML

        // Navigation Properties
        [ForeignKey("BookID")]
        public virtual Book Book { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [ForeignKey("UserID")]
        public virtual AppUser User { get; set; } // AppUser extends IdentityUser
    }
}
