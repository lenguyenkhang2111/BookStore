using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using System.Web; 
namespace BookStore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ShoppingCartController : Controller
    {
        private Data.StoreDbContext context;
        public ShoppingCartController(Data.StoreDbContext context)
    {
        this.context = context;
    }
        private readonly Services.ShoppingCartService _cartService;
        
        public ShoppingCartController(Services.ShoppingCartService cartService)
        {
            _cartService = cartService;
        }
        
        public IActionResult Index()
        {
            List<Book> cartItems = _cartService.GetCartItems();
            
            return View(cartItems);
        }
        
        public IActionResult AddToCart(int bookId)
        {
            // Lấy thông tin sách từ database dựa trên bookId
            Book book = GetBookFromDatabase(bookId);
            
            // Thêm sách vào giỏ hàng
            _cartService.AddToCart(book);
            
            return RedirectToAction("Index");
        }
        
        public IActionResult RemoveFromCart(int bookId)
        {
            // Lấy thông tin sách từ database dựa trên bookId
            Book book = GetBookFromDatabase(bookId);
            
            // Xoá sách khỏi giỏ hàng
            _cartService.RemoveFromCart(book);
            
            return RedirectToAction("Index");
        }
        
        private async Book GetBookFromDatabase(int bookId)
        {
           return await context.Books.FindAsync(bookId);
        }
    }