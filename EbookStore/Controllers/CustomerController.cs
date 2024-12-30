using EbookStore.Data; // Namespace for ApplicationDbContext
using EbookStore.Models; // Namespace for the Book model
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;

    // Constructor with Dependency Injection
    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    // MainPage Action
    public IActionResult MainPage()
    {
        // Fetch categories from the database
        var categories = _context.Categories.ToList();
        // Call the function to get the latest 4 books
        var latestBooks = GetLatestBooks(4);
        var books = _context.Books.ToList();
        ViewBag.BookCount = books.Count;
        // Fetch discounted books
        var discountedBooks = _context.Books
            .Where(b => b.IsDiscounted && b.DiscountEndDate > DateTime.Now)
            .ToList();

        // Pass data to the view
        ViewBag.Categories = categories;
        ViewBag.DiscountedBooks = discountedBooks;

        ViewBag.Books = latestBooks;

        return View();
    }
    public IActionResult PersonalLibrary()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(); // Ensure the user is logged in
        }

        var purchasedBooks = _context.OrderItems
            .Include(oi => oi.Book) // Include book details
            .Where(oi => oi.Order.UserID == userId) // Filter by the current user's orders
            .Select(oi => oi.Book) // Select only the books
            .Distinct() // Ensure no duplicate books
            .ToList();

        return View(purchasedBooks);
    }


    // Function to fetch the latest books
    private List<Book> GetLatestBooks(int count)
    {
        return _context.Books
            .OrderByDescending(b => b.CreatedDate)
            .Take(count)
            .ToList();
    }
    [HttpPost]
    [Authorize]
    public IActionResult BuyBook(int bookId)
    {
        // Logic to buy the book
        return RedirectToAction("MainPage");
    }

    [HttpPost]
    [Authorize]
    public IActionResult SendMessageToAdmin(string message)
    {
        // Logic to send a message to the admin
        return RedirectToAction("Contact");
    }

    public IActionResult BooksByCategory(int id) // id = CategoryId
    {
        var category = _context.Categories
                               .Include(c => c.Books) // Eager load related books
                               .FirstOrDefault(c => c.Id == id);

        if (category == null)
            return NotFound();

        return View(category); // Pass the category and books to the view
    }


}
