namespace EbookStore.Models
{
    public class Borrowing
    {
        public int Id { get; set; } // Primary Key
        public int BookId { get; set; } // Foreign Key to Books
        public string UserId { get; set; } // Foreign Key to Users
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }

        // Navigation properties
        public Book Book { get; set; }
    }


}
