using System.Linq;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly StoreDbContext _context;

        public CartController(StoreDbContext context)
        {
            _context = context;
        }

        // Thêm vào giỏ hàng
        public IActionResult AddToCart(int bookId, int quantity)
        {
            var book = _context.Books.Find(bookId);
            if (book == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            var cartItem = cart.CartItems.FirstOrDefault(item => item.BookId == bookId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    BookId = bookId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _context.CartItems.Update(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // Xóa giỏ hàng
        public IActionResult ClearCart()
        {
            var cart = GetCart();
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // Xóa khỏi giỏ hàng
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // Tăng số lượng
        public IActionResult IncreaseQuantity(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity++;
            _context.CartItems.Update(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // Giảm số lượng
        public IActionResult ReduceQuantity(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _context.CartItems.Update(cartItem);
            }
            else
            {
                _context.CartItems.Remove(cartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // Lấy giỏ hàng của người dùng hiện tại
        private Cart GetCart()
        {
            var userId = _context.Users.FirstOrDefault(user => user.UserName == User.Identity.Name)?.Id;

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.User.Id == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    User = _context.Users.Find(userId)
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            return cart;
        }
        // Lấy tổng số lượng sách trong giỏ hàng
        private int GetTotal()
        {
            var cart = GetCart();
            if (cart != null)
            {
                return cart.CartItems.Sum(item => item.Quantity);
            }
            return 0;
        }
        // Tiếp tục mua sắm
        public IActionResult ContinueShopping()
        {
            return RedirectToAction("Index", "Products");
        }
    }
}