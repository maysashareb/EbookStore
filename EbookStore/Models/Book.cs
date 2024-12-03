namespace EbookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; } // Nullable
        public string? Description { get; set; } // Nullable
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; } // Nullable
        public int Quantity { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedDate { get; set; } // Field to track creation date

    }
}