using Microsoft.AspNetCore.Mvc;
using EbookStore.Data;
using EbookStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EbookStore.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CreateOrder: Fetch order details from the database
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || request.OrderId == 0)
            {
                return BadRequest(new { success = false, message = "Invalid order ID." });
            }

            var order = _context.Orders
                .Where(o => o.OrderID == request.OrderId)
                .Select(o => new
                {
                    o.OrderID,
                    o.TotalAmount
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found." });
            }

            // Respond with PayPal-compatible order details
            return Ok(new
            {
                success = true,
                orderId = order.OrderID.ToString(), // Order ID as string for PayPal
                totalAmount = order.TotalAmount    // Amount to be paid
            });
        }

        // CaptureOrder: Update the database after successful payment
        [HttpPost]
        public IActionResult CaptureOrder([FromBody] CaptureOrderRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId))
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            var orderId = int.Parse(request.OrderId);
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found." });
            }

            // Simulate PayPal payment capture and update order status
            order.PaymentStatus = "Confirmed";
            _context.SaveChanges();

            return Ok(new { success = true, message = "Payment successfully captured." });
        }
    }

    // Request models
    public class CreateOrderRequest
    {
        public int OrderId { get; set; }
    }

    public class CaptureOrderRequest
    {
        public string OrderId { get; set; }
    }
}
