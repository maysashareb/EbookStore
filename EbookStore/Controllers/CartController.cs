using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EbookStore.Data;
using EbookStore.Models;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Globalization;

namespace EbookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _clientId = "AV8bBCDpoCX38F6pEGoVIh5H8aFnjpg7wgqcr75-N3aFBZlWUPIdzDoH36kWxICqWa4nGsR2PgGNTV";
        private readonly string _clientSecret = "EDtGhU509jfIRWjq4n2zqgbloZKUi0gm4yDYqRtfLNgqmrpprH_Gv_-_-SvU_5rixZbXNFx15bImtzep";
        private readonly string _baseUrl = "https://api.sandbox.paypal.com";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AddToCart(int bookId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var book = _context.Books.Find(bookId);
            if (book == null || book.AvailableCopies == 0)
            {
                return BadRequest("This book is unavailable.");
            }

            var cart = GetActiveCart(userId);
            if (cart == null)
            {
                cart = new Cart { UserID = userId, IsActive = true, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            // Check if the book already exists in the cart
            var existingCartItem = _context.CartItems.FirstOrDefault(ci => ci.CartID == cart.CartID && ci.BookId == bookId);
            if (existingCartItem != null)
            {
                return BadRequest("This book is already in your cart.");
            }

            // Add the book to the cart
            _context.CartItems.Add(new CartItems
            {
                CartID = cart.CartID,
                BookId = bookId,
                Price = book.Price
            });
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cart = GetActiveCartWithItems(userId);
            if (cart == null)
                return View(new List<CartItems>());

            return View(cart.CartItems.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int bookId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cartItem = _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefault(ci => ci.BookId == bookId && ci.Cart.UserID == userId && ci.Cart.IsActive);

            if (cartItem == null)
                return NotFound("Item not found in your cart.");

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> CheckoutWithPayPal(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return BadRequest("Order ID is required for PayPal.");

            // Simulate PayPal payment capture logic
            var paymentSuccessful = await CapturePayPalOrder(orderId);
            if (!paymentSuccessful)
                return BadRequest("PayPal payment failed.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cart = GetActiveCartWithItems(userId);

            var order = CreateOrder(cart, "Confirmed", "PayPal");
            if (order == null)
                return BadRequest("An error occurred while processing your order.");

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }
        [HttpPost]
        public IActionResult CheckoutWithCreditCard(string cardName, string cardNumber, string expiryDate, string cvv)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            if (!ValidateCreditCardInput(cardName, cardNumber, expiryDate, cvv))
            {
                return BadRequest("Invalid credit card details.");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // Retrieve user's cart
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Book)
                    .FirstOrDefault(c => c.UserID == userId && c.IsActive);

                if (cart == null || !cart.CartItems.Any())
                {
                    throw new InvalidOperationException("Your cart is empty or invalid.");
                }

                // Ensure no duplicate books in the cart
                if (cart.CartItems.GroupBy(ci => ci.BookId).Any(g => g.Count() > 1))
                {
                    throw new InvalidOperationException("Your cart contains duplicate books. Please ensure each book is added only once.");
                }

                // Calculate total amount
                decimal totalAmount = cart.CartItems.Sum(ci => ci.Price);

                // Create the order
                var order = new Order
                {
                    UserID = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    PaymentStatus = "Confirmed",
                    PaymentMethod = "CreditCard"
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Process each cart item
                foreach (var cartItem in cart.CartItems)
                {
                    var book = _context.Books.Find(cartItem.BookId);

                    // Validate book availability
                    if (book == null || book.AvailableCopies < 1)
                    {
                        throw new InvalidOperationException($"Book '{book?.Title}' is out of stock.");
                    }

                    // Update inventory
                    book.AvailableCopies -= 1;

                    // Add order item
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderID = order.OrderID,
                        BookId = cartItem.BookId,
                        Quantity = 1, // Only one book of each type is allowed
                        Price = cartItem.Price
                    });

                    // Add to user's library
                    _context.UserLibrary.Add(new UserLibrary
                    {
                        UserID = userId,
                        BookID = cartItem.BookId,
                        PurchaseDate = DateTime.Now,
                        OrderID = order.OrderID,
                        Format = "PDF,EPUB,MOBI,HTML"
                    });
                }

                // Deactivate the cart
                cart.IsActive = false;
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();

                transaction.Commit();

                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new
                {
                    Error = ex.GetType().Name,
                    Details = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        private bool ValidateCreditCardInput(string cardName, string cardNumber, string expiryDate, string cvv)
        {
            if (string.IsNullOrWhiteSpace(cardName) ||
                string.IsNullOrWhiteSpace(cardNumber) ||
                string.IsNullOrWhiteSpace(expiryDate) ||
                string.IsNullOrWhiteSpace(cvv))
                return false;

            // Remove spaces and non-numeric characters
            cardNumber = Regex.Replace(cardNumber, @"\D", "");
            cvv = Regex.Replace(cvv, @"\D", "");

            // Validate card number (must be 16 digits)
            if (!Regex.IsMatch(cardNumber, @"^\d{16}$"))
                return false;

            // Validate CVV (must be 3 digits)
            if (!Regex.IsMatch(cvv, @"^\d{3}$"))
                return false;

            // Validate expiry date
            if (!DateTime.TryParseExact(expiryDate,
                new[] { "MM/yyyy", "MM/yy", "yyyy-MM" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime expiry))
                return false;

            // Check if card is not expired
            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return expiry >= currentDate;
        }


        private Cart GetActiveCart(string userId)
        {
            return _context.Carts.FirstOrDefault(c => c.UserID == userId && c.IsActive);
        }


        private Cart GetActiveCartWithItems(string userId)
        {
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.UserID == userId && c.IsActive);

            if (cart == null)
            {
                Console.WriteLine($"[Debug] No active cart found for userId: {userId}");
            }
            else
            {
                Console.WriteLine($"[Debug] Cart ID: {cart.CartID}, Items Count: {cart.CartItems?.Count ?? 0}");
                foreach (var item in cart.CartItems)
                {
                    Console.WriteLine($"[Debug] CartItem - BookID: {item.BookId}, Quantity: {item.Quantity}");
                }
            }

            return cart;
        }


        private bool CartContainsBook(int cartId, int bookId)
        {
            return _context.CartItems.Any(ci => ci.CartID == cartId && ci.BookId == bookId);
        }

        private Order CreateOrder(Cart cart, string paymentStatus, string? paymentMethod)
        {
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty or invalid.");

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // First verify all books are available
                foreach (var item in cart.CartItems)
                {
                    var book = _context.Books.Find(item.BookId);
                    if (book == null || book.AvailableCopies < item.Quantity)
                        throw new InvalidOperationException($"Book '{book?.Title}' is not available in the requested quantity.");
                }

                // Create the order
                var order = new Order
                {
                    UserID = cart.UserID,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity), // Use the price from CartItems
                    PaymentStatus = paymentStatus,
                    PaymentMethod = paymentMethod
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Create order items
                var orderItems = cart.CartItems.Select(item => new OrderItem
                {
                    OrderID = order.OrderID,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Price // Use the price from CartItem
                }).ToList();

                _context.OrderItems.AddRange(orderItems);

                // Update book quantities
                foreach (var item in cart.CartItems)
                {
                    var book = _context.Books.Find(item.BookId);
                    book.AvailableCopies -= item.Quantity;
                }

                // Clear the cart
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.IsActive = false;

                _context.SaveChanges();
                transaction.Commit();
                return order;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in CreateOrder: {ex.Message}");
                throw;
            }
        }




        private async Task<bool> CapturePayPalOrder(string orderId)
        {
            // Simulate PayPal capture logic; replace with actual PayPal integration
            await Task.Delay(100); // Simulating async API call
            return true; // Assume the payment is successful
        }

        public IActionResult Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.UserID == userId && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("Your cart is empty.");
            }

            var order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.CartItems.Sum(ci => ci.Price),
                PaymentStatus = "Pending",
                PaymentMethod = "CreditCard"
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var cartItem in cart.CartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    BookId = cartItem.BookId,
                    Price = cartItem.Price
                });
            }

            // Clear the cart
            cart.IsActive = false;
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.SaveChanges();

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            try
            {
                // Retrieve the cart for the current user
                var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
                if (userId == null)
                {
                    return Json(0); // Return 0 if the user is not authenticated
                }

                var cart = _context.Carts.Include(c => c.CartItems)
                                         .FirstOrDefault(c => c.UserID == userId);

                // Return the count of items in the cart
                var cartCount = cart?.CartItems.Sum(item => item.Quantity) ?? 0;
                return Json(cartCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching cart count: " + ex.Message);
                return Json(0);
            }
        }



    }
}
