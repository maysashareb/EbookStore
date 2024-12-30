namespace EbookStore.Models
{
    public class WaitingList
    {
        public int Id { get; set; } // Primary Key
        public int BookId { get; set; } // Foreign Key to Books
        public string UserId { get; set; } // Foreign Key to Users
        public int Position { get; set; } // Position in the waiting list
        public DateTime DateAdded { get; set; } // Date when user was added to the waiting list

        public DateTime? BorrowedDate { get; set; }  // Optional: Date when the book was borrowed
        public DateTime? DueDate { get; set; }      // Optional: Date when the book is due
        public bool SendReminder { get; set; }

        // Navigation properties
        public Book Book { get; set; }
        public AppUser User { get; set; }  // Navigation property to User (replace with your actual User class)

    }


}
