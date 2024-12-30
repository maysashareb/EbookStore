using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookStore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; } // Primary key
        [Required]
        public string? Title { get; set; } // Nullable
        public string? Description { get; set; } // Nullable


        [Required,DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BorrowPrice { get; set; } 
        public bool IsBorrowable { get; set; }
        public int CategoryId { get; set; } // Keep this for the foreign key in Books
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public string? ImageUrl { get; set; } // Nullable
        public int Quantity { get; set; }
        // Navigation property for OrderItems
        public ICollection<OrderItem> OrderItems { get; set; }
        public DateTime CreatedDate { get; set; } // This is non-nullable
        [Required]
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public bool IsDiscounted { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? EpubUrl { get; set; }
        public string? Fb2Url { get; set; }
        public string? MobiUrl { get; set; }
        public string? PdfUrl { get; set; }
        public int? AvailableCopiesBorrow { get; set; }
        public int PublisheYear { get; set; }
        [ConcurrencyCheck]
        public int AvailableCopies { get; set; }

        public decimal? AverageRating { get; set; }
        public int? RatingCount { get; set; }


        public int agelimt { get; set; }

        public virtual ICollection<CartItems> CartItems { get; set; } // Navigation property


    }
}