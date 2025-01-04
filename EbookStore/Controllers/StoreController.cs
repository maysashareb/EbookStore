using EbookStore.Data;
using EbookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EbookStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }
        //public IActionResult Gallery()
        //{
        //    var books = _context.Books.ToList();
        //    return View(books);

        //}

        public IActionResult Gallery(string searchType, string searchQuery, string genreFilter, string yearFilter, string sortOrder)
        {
            // Fetch books with category included
            var books = _context.Books.Include(b => b.Category).AsQueryable();

            // Get all categories
            ViewBag.Categories = _context.Categories.ToList();
            Console.WriteLine($"Initial Books Count: {books.Count()}");

            // Search logic
            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchType == "Title")
                {
                    books = books.Where(b => b.Title.Contains(searchQuery));
                }
                else if (searchType == "Author")
                {
                    books = books.Where(b => b.Author.Contains(searchQuery));
                }
                Console.WriteLine($"Books Count After Search: {books.Count()}");
            }

            // Genre filter
            if (!string.IsNullOrEmpty(genreFilter))
            {
                books = books.Where(b => b.Category.Name == genreFilter);
                Console.WriteLine($"Books Count After Genre Filter: {books.Count()}");
            }

            // Year filter
            if (!string.IsNullOrEmpty(yearFilter) && int.TryParse(yearFilter, out int year))
            {
                books = books.Where(b => b.PublisheYear == year);
                Console.WriteLine($"Books Count After Year Filter: {books.Count()}");
            }

            // Sorting logic
            switch (sortOrder)
            {
                case "price_asc":
                    books = books.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.Price);
                    break;
                case "year":
                    books = books.OrderByDescending(b => b.PublisheYear);
                    break;
                case "category":
                    books = books.OrderBy(b => b.Category.Name);
                    break;
                case "author":
                    books = books.OrderBy(b => b.Author);
                    break;
                case "title":
                default:
                    books = books.OrderBy(b => b.Title);
                    break;
            }

            Console.WriteLine($"Final Books Count: {books.Count()}");

            // Return the view with the books
            return View(books.ToList());
        }


        public IActionResult Details(int id)
        {
            var book = _context.Books
                .Include(b => b.Category) // Eagerly load the related Category
                .FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var waitingListCount = _context.WaitingLists
                .Where(w => w.BookId == id)
                .Count(); // Get the number of people waiting for this book

            var nextAvailableDate = _context.Borrowings
                .Where(b => b.BookId == id && b.ReturnDate == null)
                .OrderBy(b => b.BorrowDate)
                .Select(b => b.BorrowDate.AddDays(30)) // Assume books are borrowed for 30 days
                .FirstOrDefault(); // Get the next available date for the book

            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                WaitingListCount = waitingListCount,
                NextAvailableDate = nextAvailableDate
            };

            return View(viewModel);
        }

    }
}
