using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; } // Nullable
        public string? Description { get; set; } // Nullable

        [Required,DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public decimal BorrowPrice { get; set; } 
        public bool IsBorrowable { get; set; }
        public int CategoryId { get; set; } // Keep this for the foreign key in Books
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public string? ImageUrl { get; set; } // Nullable
        public int Quantity { get; set; }

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
        public int AvailableCopies { get; set; }

        public int agelimt { get; set; }



    }
}