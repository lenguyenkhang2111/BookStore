using System;
using System.Collections.Generic;
using System.Linq;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(StoreDbContext context, UserManager<User> userManager,
             IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

        }
        public ViewResult Index()
        {
            Cart cart = GetCart();

            return View("/Views/Order/Index.cshtml", cart);
        }

        // Tạo đơn hàng
        public IActionResult CreateOrder()
        {
            Cart cart = GetCart();

            if (cart.CartItems.Count == 0)
            {
                // Khi giỏ hàng trống, không thể tạo đơn hàng
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserId = cart.UserId,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    Order = null, // Set Order property to null here
                    Book = null,
                    BookId = ci.BookId,
                    Quantity = ci.Quantity
                }).ToList()
            };

            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Order = order; // Assign the order to each order item
                orderItem.Book = _context.Books.Find(orderItem.BookId);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Xóa giỏ hàng sau khi tạo đơn hàng
            ClearCart();

            return RedirectToAction("OrderHistory");
        }

        // Thanh toán
        public IActionResult CheckOut()
        {
            var cart = GetCart();

            if (cart?.CartItems?.Count == 0)
            {
                // Khi giỏ hàng trống, không thể thanh toán
                return RedirectToAction("Index", "Cart");
            }

            // Thực hiện các bước thanh toán 

            // Đánh dấu đơn hàng đã thanh toán
            MarkOrderAsPaid();

            return RedirectToAction("CheckOutComplete");
        }

        // Hoàn thành thanh toán
        public IActionResult CheckOutComplete()
        {
            return View();
        }

        // Xóa giỏ hàng
        private void ClearCart()
        {
            Cart cart = GetCart();
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.SaveChanges();
        }

        // Đánh dấu đơn hàng đã thanh toán
        private void MarkOrderAsPaid()
        {
            Cart cart = GetCart();
            var order = _context.Orders.Include(o => o.OrderItems)
                                       .FirstOrDefault(o => o.UserId == cart.UserId && o.OrderItems.Any());

            if (order != null)
            {
                order.OrderDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        // Lấy giỏ hàng của người dùng hiện tại
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
        public IActionResult OrderHistory()
        {
            var userId = GetUserId();
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Where(o => o.UserId == userId)
                .ToList();

            return View(orders);
        }
    }
}