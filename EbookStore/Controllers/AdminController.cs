using EbookStore.Data;
using EbookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        ViewBag.Categories = _context.Categories.ToList(); // Populate dropdown
        return View();
    }

    [HttpPost]
    public IActionResult ManageBooks(Book model, IFormFile bookCover)
    {
        if (ModelState.IsValid)
        {
            // Ensure a valid category is selected
            if (_context.Categories.Any(c => c.Id == model.CategoryId))
            {
                // Save the uploaded file
                if (bookCover != null && bookCover.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/Content/books", bookCover.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        bookCover.CopyTo(stream);
                    }
                    model.ImageUrl = $"/Content/books/{bookCover.FileName}";
                }

                _context.Books.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Book added successfully!";
                return RedirectToAction("ManageBooks");
            }
            else
            {
                ModelState.AddModelError("CategoryId", "Invalid category selected.");
            }
        }

        ViewBag.Categories = _context.Categories.ToList(); // Repopulate dropdown
        return View(model);
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
        var category = _context.Categories.Find(id);
        if (category != null)
        {
            // Delete the associated image file
            var imagePath = Path.Combine("wwwroot", category.Image?.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["Success"] = "Category deleted successfully!";
        }

        return RedirectToAction("ManageCategories");
    }

}




