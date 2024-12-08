using EbookStore.Data; // Namespace for ApplicationDbContext
using EbookStore.Models; // Namespace for the Book model
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

}
