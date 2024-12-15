using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EbookStore.Data;
using EbookStore.Models;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EbookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _clientId = "YourPayPalClientId";
        private readonly string _clientSecret = "YourPayPalClientSecret";
        private readonly string _baseUrl = "https://api.sandbox.paypal.com";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add to Cart
        public IActionResult AddToCart(int bookId, string transactionType)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var cart = _context.Carts.FirstOrDefault(c => c.UserID == userId && c.IsActive);
            if (cart == null)
            {
                cart = new Cart { UserID = userId, IsActive = true, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

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
                    TransactionType = transactionType ?? "DefaultType"
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
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
        public async Task<IActionResult> Checkout()
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

            var order = new EbookStore.Models.Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.CartItems.Sum(ci => ci.TransactionType == "Buy" ? ci.Book.Price * ci.Quantity : ci.Book.BorrowPrice * ci.Quantity),
                PaymentStatus = "Pending",
                PaymentMethod = "PayPal"
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

            // Redirect to PayPal for payment
            var approvalUrl = await CreatePayPalPayment(order);
            if (string.IsNullOrEmpty(approvalUrl))
            {
                return View("Error");
            }

            return Redirect(approvalUrl);
        }

        // Create PayPal Payment
        private async Task<string> CreatePayPalPayment(EbookStore.Models.Order order)
        {
            using var client = new HttpClient();
            var accessToken = await GetPayPalAccessToken();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var itemList = order.OrderItems.Select(item => new
            {
                name = item.Book.Title,
                quantity = item.Quantity.ToString(),
                unit_amount = new { currency_code = "USD", value = item.Price.ToString("F2") },
                category = "PHYSICAL_GOODS"
            }).ToList();

            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new {
                        amount = new {
                            currency_code = "USD",
                            value = order.TotalAmount.ToString("F2"),
                            breakdown = new {
                                item_total = new { currency_code = "USD", value = order.TotalAmount.ToString("F2") }
                            }
                        },
                        items = itemList
                    }
                },
                application_context = new
                {
                    return_url = Url.Action("Success", "Cart", null, Request.Scheme),
                    cancel_url = Url.Action("Cancel", "Cart", null, Request.Scheme)
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_baseUrl}/v2/checkout/orders", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);

            var links = (IEnumerable<dynamic>)result.links;
            return links.FirstOrDefault(link => link.rel == "approve")?.href;

        }

        // Get PayPal Access Token
        private async Task<string> GetPayPalAccessToken()
        {
            using var client = new HttpClient();

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await client.PostAsync($"{_baseUrl}/v1/oauth2/token", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseBody);

            return result.access_token;
        }

        // Success
        public IActionResult Success(string paymentId)
        {
            return View("Success");
        }

        // Cancel
        public IActionResult Cancel()
        {
            return View("Cancel");
        }
    }
}
