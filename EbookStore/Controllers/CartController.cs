using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EbookStore.Data;
using EbookStore.Models;
using System.Linq;
using PayPal.Api;

namespace EbookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Cancel()
        {
            // Handle the cancellation (e.g., notify the user)
            return View("Cancel");
        }

        public IActionResult AddToCart(int bookId, string transactionType)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Check if the user has an active cart
            var cart = _context.Carts.FirstOrDefault(c => c.UserID == userId && c.IsActive);
            if (cart == null)
            {
                cart = new Cart { UserID = userId, IsActive = true, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            // Add or update the cart item
            var existingItem = _context.CartItems.FirstOrDefault(ci => ci.CartID == cart.CartID && ci.BookID == bookId && ci.TransactionType == transactionType);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItems
                {
                    CartID = cart.CartID,
                    BookID = bookId,
                    Quantity = 1,
                    TransactionType = transactionType ?? "DefaultType" // Ensure TransactionType has a value
                

            };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            // Redirect to the Cart page
            return RedirectToAction("index");
        }


        public IActionResult Success(string paymentId, string token, string PayerID)
        {
            if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(PayerID))
            {
                return View("Error"); // Handle missing parameters
            }

            // Initialize PayPal API context
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            // Execute the payment
            var payment = new Payment() { id = paymentId };
            var execution = new PaymentExecution() { payer_id = PayerID };

            try
            {
                var executedPayment = payment.Execute(apiContext, execution);

                if (executedPayment.state.ToLower() != "approved")
                {
                    return View("Error"); // Payment failed
                }

                // Payment successful, redirect to a success page
                return View("Success");
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine(ex.Message);
                return View("Error");
            }
        }

        // View Cart
        public IActionResult Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.UserID == userId && c.IsActive);

            if (cart == null)
                return View(new List<CartItems>());

            return View(cart.CartItems.ToList());
        }

        // Checkout
        [HttpPost]
        [HttpPost]
        public IActionResult Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.UserID == userId && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
                return BadRequest("Your cart is empty.");

            // Use fully qualified name for the Order model
            var order = new EbookStore.Models.Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.CartItems.Sum(ci => ci.TransactionType == "Buy" ? ci.Book.Price * ci.Quantity : ci.Book.BorrowPrice * ci.Quantity),
                PaymentStatus = "Pending",
                PaymentMethod = "PayPal" // Example: can integrate payment here
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cart.CartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    BookID = item.BookID,
                    Quantity = item.Quantity,
                    Price = item.TransactionType == "Buy" ? item.Book.Price : item.Book.BorrowPrice
                });
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            cart.IsActive = false;

            _context.SaveChanges();

            return RedirectToAction("Success");
        }


        public IActionResult RemoveFromCart(int id)
        {
            var item = _context.CartItems.FirstOrDefault(ci => ci.CartItemID == id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
            // Configure PayPal settings
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            // Create payment details
            var cartItems = _context.CartItems.ToList();
            var itemList = new ItemList() { items = new List<Item>() };

            foreach (var cartItem in cartItems)
            {
                itemList.items.Add(new Item()
                {
                    name = cartItem.Book.Title,
                    currency = "USD",
                    price = cartItem.Book.Price.ToString(),
                    quantity = cartItem.Quantity.ToString(),
                    sku = cartItem.BookID.ToString()
                });
            }

            var payer = new Payer() { payment_method = "paypal" };
            var redirectUrls = new RedirectUrls()
            {
                cancel_url = Url.Action("Cancel", "Cart", null, Request.Scheme),
                return_url = Url.Action("Success", "Cart", null, Request.Scheme)
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = cartItems.Sum(c => c.Book.Price * c.Quantity).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = details.subtotal, // Total must match subtotal + tax + shipping
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "EbookStore purchase",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirectUrls
            };

            var createdPayment = payment.Create(apiContext);

            // Extract the redirect URL
            var approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.Equals("approval_url")).href;

            return Redirect(approvalUrl);
           

        }
    }

}
