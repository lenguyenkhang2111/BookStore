using System;
using System.Collections.Generic;
using System.Linq;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers
{
    [Authorize]
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

            return View(cart);
        }

        public IActionResult CreateOrder()
        {
            Cart cart = GetCart();

            if (cart.CartItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserId = cart.UserId,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    Order = null,
                    Book = null,
                    BookId = ci.BookId,
                    Quantity = ci.Quantity
                }).ToList()
            };

            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Order = order;
                orderItem.Book = _context.Books.Find(orderItem.BookId);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            ClearCart();

            return RedirectToAction("OrderHistory");
        }

        // Thanh toán
        public IActionResult CheckOut()
        {
            var cart = GetCart();

            if (cart?.CartItems?.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }


            MarkOrderAsPaid();

            return RedirectToAction("CheckOutComplete");
        }

        public IActionResult CheckOutComplete()
        {
            return View();
        }

        private void ClearCart()
        {
            Cart cart = GetCart();
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.SaveChanges();
        }

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