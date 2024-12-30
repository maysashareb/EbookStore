using System;

namespace EbookStore.Models
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; } // The book itself
        public int WaitingListCount { get; set; } // Number of people in the waiting list
        public DateTime? NextAvailableDate { get; set; } // Date when the book is next available
        public bool CanBorrow { get; set; } // Whether the current user can borrow the book

        public bool HasNoCopies { get; set; }


    }
}
