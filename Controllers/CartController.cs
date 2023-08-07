using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Controllers
{
    [Route("/cart")]
    public class CartController : Controller
    {
        private readonly Data.StoreDbContext context;

        public CartController(Data.StoreDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var books = context.Books.ToList();
            return View(books);
        }

        [Route("addcart/{bookid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute]int bookid)
        {
            var book = context.Books
                .FirstOrDefault(b => b.Id == bookid);

            if (book == null)
                return NotFound("Empty");

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(b => b.Id == bookid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.Quantity++;
            }
            else
            {
                // Thêm mới
                cart.Add(new CartItem() { Quantity = 1, Book = book });
            }
            // Lưu cart vào Session
            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        /// xóa item trong cart
        [Route("/removecart/{bookid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute]int bookid)
        {
            // Xử lý xóa một mục của Cart ...
            return RedirectToAction(nameof(Cart));
        }

        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm]int bookid, [FromForm]int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...

            return RedirectToAction(nameof(Cart));
        }

        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View();
        }

        [Route("/checkout")]
        public IActionResult CheckOut()
        {
            // Xử lý khi đặt hàng
            return View();
        }

        // Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";

        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
    }

    
}