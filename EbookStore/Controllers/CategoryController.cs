using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EbookStore.Data;
using EbookStore.Models;
using Microsoft.AspNetCore.Authorization;
namespace EbookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        public IActionResult Edit(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpPost]

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id); // Find the category
            if (category != null)
            {
                _context.Categories.Remove(category); // Remove it from the database
                _context.SaveChanges(); // Commit the changes
                TempData["Success"] = "Category deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Category not found.";
            }
            return RedirectToAction("ManageCategories");
        }


    }
}

