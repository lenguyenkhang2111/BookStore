using System;
using System.Collections.Generic;
using System.Linq;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly StoreDbContext _context;

        public OrderController(StoreDbContext context)
        {
            _context = context;
        }

        // Tạo đơn hàng
        public IActionResult CreateOrder()
        {
            var cart = GetCart();

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

            return RedirectToAction("Index", "Home");
        }

        // Thanh toán
        public IActionResult CheckOut()
        {
            var cart = GetCart();

            if (cart.CartItems.Count == 0)
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
            var cart = GetCart();
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.SaveChanges();
        }

        // Đánh dấu đơn hàng đã thanh toán
        private void MarkOrderAsPaid()
        {
            var cart = GetCart();
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
            var userId = _context.Users.FirstOrDefault(user => user.UserName == User.Identity.Name)?.Id.ToString();

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefault(c => c.UserId == int.Parse(userId));

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = int.Parse(userId),
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            return cart;
        }
    }
}