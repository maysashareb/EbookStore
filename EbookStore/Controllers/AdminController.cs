using EbookStore.Data;
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

    public IActionResult Dashboard()
    {
        var categories = _context.Categories.ToList();
        var books = _context.Books.ToList();

        ViewData["Categories"] = categories;
        ViewData["Books"] = books;

        return View();
    }
}
