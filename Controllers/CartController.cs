using System.Linq;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BookStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(StoreDbContext context, UserManager<User> userManager,
             IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public ViewResult Index()
        {
            Cart cart = GetCart();

            return View(cart);
        }

        public IActionResult AddToCart(int bookID, int quantity)
        {
            var book = _context.Books.Find(bookID);
            if (book == null)
            {
                return NotFound();
            }

            Cart cart = GetCart();

            var cartItem = cart?.CartItems?.FirstOrDefault(item => item.BookId == bookID);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Cart = cart!,
                    Book = book,
                    Quantity = quantity + 1
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
                _context.CartItems.Update(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            var cart = GetCart();
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult ReduceQuantity(int cartItemId)
        {
            var cartItem = _context.CartItems?.Find(cartItemId);

            if (cartItem != null)
            {
                if (cartItem?.Quantity > 1)
                {
                    cartItem.Quantity--;
                    _context.CartItems?.Update(cartItem);
                }
                else
                {
                    _context.CartItems?.Remove(cartItem);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index"); ;
        }

        public IActionResult IncreaseQuantity(int cartItemId)
        {
            var cartItem = _context.CartItems?.Find(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                _context.CartItems.Update(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        private Cart GetCart()
        {
            var userId = GetUserId();

            Cart cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            return cart;

        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor?.HttpContext?.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
        private int GetTotal()
        {
            var cart = GetCart();
            if (cart != null)
            {
                return cart.CartItems.Sum(item => item.Quantity);
            }
            return 0;
        }
        public IActionResult ContinueShopping()
        {
            return View("/Views/Book/Index.cshtml");
        }
    }
}