using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EbookStore.Data; // Namespace for DbContext
using EbookStore.Models; // Namespace for models

using Microsoft.AspNetCore.Authorization;


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

        // GET: /Book/SearchBooks
        [HttpGet]
        public JsonResult SearchBooks(string searchTerm)
        {
            var books = _context.Books
                .Where(b => b.Title.Contains(searchTerm)) // Searches by title
                .Select(b => new { b.Id, b.Title }) // Return only necessary fields
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
    }
}
    
