using EbookStore.Data;
using EbookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Managebooks()
    {
        return View();
    }
    public IActionResult Home()
    {
        var categories = _context.Categories.ToList();
        var books = _context.Books.ToList();

        ViewData["Categories"] = categories;
        ViewData["Books"] = books;

        return View();
    }


    [HttpGet]
    public IActionResult ManageBooks()
    {
        // Retrieve categories for dropdown
        ViewBag.Categories = _context.Categories.ToList();

        // Retrieve all books to display in the list
        var books = _context.Books.ToList();

        // Pass the books to the view
        return View(books);
    }

    [HttpPost]
    public IActionResult ManageBooks(Book model, IFormFile bookCover)
    {
        if (ModelState.IsValid)
        {
            // Validate the selected category exists
            if (_context.Categories.Any(c => c.Id == model.CategoryId))
            {
                if (bookCover != null && bookCover.Length > 0)
                {
                    var uploadDirectory = Path.Combine("wwwroot", "Content", "books");
                    Directory.CreateDirectory(uploadDirectory);
                    var filePath = Path.Combine(uploadDirectory, bookCover.FileName);

                    // Save the uploaded file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        bookCover.CopyTo(stream);
                    }
                    model.ImageUrl = $"/Content/books/{bookCover.FileName}";
                }

                // Set the created date for the book
                model.CreatedDate = DateTime.Now;

                // Add the new book to the database
                _context.Books.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Book added successfully!";
            }
            else
            {
                ModelState.AddModelError("CategoryId", "Invalid category selected.");
            }
        }

        // Repopulate ViewBag for dropdown and reload the books list
        ViewBag.Categories = _context.Categories.ToList();
        var books = _context.Books.ToList();

        // Pass updated books list to the view
        return View(books);
    }
    [HttpGet]
    public IActionResult AllOrders()
    {
        // Fetch all orders with related data
        var orders = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .OrderByDescending(o => o.OrderDate) // Optional: sort by most recent
            .ToList();

        return View(orders);
    }

    [HttpGet]
    public IActionResult ManageCategories()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    [HttpPost]
    public IActionResult AddCategory(Category model, IFormFile categoryImage)
    {
        if (ModelState.IsValid)
        {
            // Save the uploaded image
            if (categoryImage != null && categoryImage.Length > 0)
            {
                var filePath = Path.Combine("wwwroot/Content", categoryImage.FileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure the directory exists
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    categoryImage.CopyTo(stream);
                }
                model.Image = $"/Content/{categoryImage.FileName}";
            }

            // Save the category to the database
            _context.Categories.Add(model);
            _context.SaveChanges();
            TempData["Success"] = "Category added successfully!";
        }

        return RedirectToAction("ManageCategories");
    }

   
    [HttpPost]
    public IActionResult DeleteCategory(int id)
    {
        // Find the category and its dependent records
        var category = _context.Categories.Find(id);

        if (category != null)
        {
            // Delete related books
            var booksToDelete = _context.Books.Where(b => b.CategoryId == id).ToList();

            foreach (var book in booksToDelete)
            {
                // Delete related OrderItems for each book
                var orderItems = _context.OrderItems.Where(oi => oi.BookId== book.Id).ToList();
                _context.OrderItems.RemoveRange(orderItems);

                _context.Books.Remove(book); // Delete the book
            }

            // Finally, delete the category
            _context.Categories.Remove(category);

            _context.SaveChanges(); // Commit changes
            TempData["Success"] = "Category deleted successfully!";
        }
        else
        {
            TempData["Error"] = "Category not found.";
        }

        return RedirectToAction("ManageCategories");
    }

    public IActionResult Messages()
    {
        var messages = _context.Messages.OrderByDescending(m => m.SentAt).ToList();
        return View(messages);
    }
    [HttpGet]
    public IActionResult OrderDetails(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefault(o => o.OrderID == orderId);

        if (order == null)
            return NotFound("Order not found.");

        return View(order);
    }

}




