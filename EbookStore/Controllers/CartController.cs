using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using EbookStore.Models;

namespace EbookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View Cart
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // Add Item to Cart
        [HttpPost]
        public IActionResult AddToCart(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.BookId == bookId);
            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    Quantity = 1
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["Success"] = $"{book.Title} has been added to your cart.";
            return RedirectToAction("Index", "Books");
        }

        // Remove Item from Cart
        public IActionResult RemoveFromCart(int bookId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.BookId == bookId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Success"] = "Item removed from your cart.";
            }

            return RedirectToAction("Index");
        }

        // Update Quantity in Cart
        [HttpPost]
        public IActionResult UpdateCart(int bookId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.BookId == bookId);
            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
            }
            else if (cartItem != null && quantity <= 0)
            {
                cart.Remove(cartItem);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // Checkout
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            // Logic for processing the cart (e.g., payment, order creation, etc.)
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Index", "Books");
        }
    }
}
