using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EbookStore.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Range(1, 5)]
        public int Value { get; set; }

        public Book Book { get; set; }
        public string Comment { get; set; } // User's comment
        public string PhotoPath { get; set; } // Path for uploaded photo
        public DateTime SubmittedAt { get; set; } // Date submitted

    }
}