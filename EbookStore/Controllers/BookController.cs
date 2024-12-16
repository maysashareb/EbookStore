using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EbookStore.Data; // Namespace for DbContext
using EbookStore.Models; // Namespace for models

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace EbookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display all books
        public IActionResult Index()
        {
            var books = _context.Books.ToList();
            return View(books);

            var booksOnSale = _context.Books
        .Where(b => b.DiscountPrice.HasValue && b.DiscountEndDate >= DateTime.Now)
        .ToList();

            var allBooks = _context.Books.ToList();

            ViewBag.Books = allBooks;
            ViewBag.DiscountedBooks = booksOnSale;

        }

        // Display details of a specific book
        public IActionResult Details(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        // Create a new book (GET)
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList(); // To display category options
            return View();
        }

        // Create a new book (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList(); // Rebind categories if model validation fails
            return View(book);
        }

        // Edit a book (GET)
        public IActionResult Edit(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList(); // To display category options
            return View(book);
        }

        // Edit a book (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // Delete a book
        public IActionResult Delete(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        // Allow search for both authenticated and non-authenticated users
        [AllowAnonymous]

        [HttpGet]
        public IActionResult SearchBooks(string query)
        {
            // Make sure the query is not empty
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<Book>());
            }

            // Search books by title, author, or publisher
            var books = _context.Books
                .Where(b => b.Title.Contains(query) || b.Author.Contains(query) || b.Publisher.Contains(query))
                .Select(b => new { b.Id, b.Title, b.Author, b.Publisher })
                .ToList();

            return Json(books);
        }

        // GET: /Book/BookDetails/{id}
        public IActionResult BookDetails(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound(); // Proper ASP.NET Core error response
            }
            return View(book); // Return a view to display book details
        }

        // Get discounted books
        public IActionResult Sales()
        {
            var discountedBooks = _context.Books
                .Where(b => b.IsDiscounted && b.DiscountEndDate > DateTime.Now)
                .ToList();
            return View(discountedBooks);
        }

        // Automatically remove expired discounts
        [HttpPost]
        public IActionResult RemoveExpiredDiscounts()
        {
            var expiredBooks = _context.Books
                .Where(b => b.IsDiscounted && b.DiscountEndDate <= DateTime.Now)
                .ToList();

            foreach (var book in expiredBooks)
            {
                book.IsDiscounted = false;
                book.DiscountPrice = null;
                book.DiscountEndDate = null;
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Sales));
        }
        [AllowAnonymous] // Make it accessible to all users
        public IActionResult Gallery()
        {
            var books = _context.Books.Include(b => b.Category).ToList(); // Fetch books with categories
            return View(books); // Pass the list of books to the view
        }
    }
}


