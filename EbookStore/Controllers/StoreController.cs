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
        //public IActionResult Gallery()
        //{
        //    var books = _context.Books.ToList();
        //    return View(books);

        //}

        public IActionResult Gallery(string searchType, string searchQuery, string genreFilter, string yearFilter, string sortOrder)
        {
            var books = _context.Books.Include(b => b.Category).AsQueryable();

            // Get all categories
            ViewBag.Categories = _context.Categories.ToList();

            // Check if searchQuery is not empty and determine search type
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
            }

            // Apply genre filter if selected
            if (!string.IsNullOrEmpty(genreFilter))
            {
                books = books.Where(b => b.Category.Name == genreFilter);
            }

            // Apply year filter if selected
            if (!string.IsNullOrEmpty(yearFilter) && int.TryParse(yearFilter, out int year))
            {
                books = books.Where(b => b.PublisheYear == year);
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

            return View(books.ToList());
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
