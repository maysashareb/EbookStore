using EbookStore.Data;
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
        public IActionResult Gallery()
        {
            var books = _context.Books.ToList();
            return View(books);

        }
        
        public IActionResult Details(int id)
        {
            // Retrieve the book, including its associated Category
            var book = _context.Books
                .Include(b => b.Category) // Eagerly load the related Category
                .FirstOrDefault(b => b.Id == id);

            // Handle the case where the book isn't found
            if (book == null)
            {
                return NotFound();
            }

            // Pass the book to the view
            return View(book);
        }

    }
}
