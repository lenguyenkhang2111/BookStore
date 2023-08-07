using BookStore.Models;

namespace BookStore.Services
{
    public class ShoppingCartService
    {
        private readonly List<Book> _booksInCart;
        
        public ShoppingCartService()
        {
            _booksInCart = new List<Book>();
        }
        
        public void AddToCart(Book book)
        {
            _booksInCart.Add(book);
        }
        
        public void RemoveFromCart(Book book)
        {
            _booksInCart.Remove(book);
        }
        
        public List<Book> GetCartItems()
        {
            return _booksInCart;
        }
        
        public void ClearCart()
        {
            _booksInCart.Clear();
        }

    }
}