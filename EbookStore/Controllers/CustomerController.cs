using EbookStore.Data; // Namespace for ApplicationDbContext
using EbookStore.Models; // Namespace for the Book model
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


        // Pass data to the view
        ViewBag.Categories = categories;
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
}
