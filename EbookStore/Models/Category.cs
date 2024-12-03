namespace EbookStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; } // Nullable
        public string? Image { get; set; } // Nullable
                                          
        public ICollection<Book>? Books { get; set; }
    }
}