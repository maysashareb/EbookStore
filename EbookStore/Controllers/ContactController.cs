using EbookStore.Data;
using EbookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace EbookStore.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Send(string Name, string Email, string Message)
        {
            var userMessage = new Message
            {
                Name = Name,
                Email = Email,
                Content = Message,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(userMessage);
            _context.SaveChanges();

            TempData["Success"] = "Your message has been sent!";
            return RedirectToAction("Mainpage", "Customer");
        }
    }
}
