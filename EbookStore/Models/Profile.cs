using EbookStore.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EbookStore.Models
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Book> PersonalLibrary { get; set; }

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch purchased books for the logged-in user
            PersonalLibrary = _context.Orders
                .Where(o => o.UserID == userId)
                .SelectMany(o => o.OrderItems)
                .Select(oi => oi.Book)
                .Distinct()
                .ToList();
        }
    }

}
