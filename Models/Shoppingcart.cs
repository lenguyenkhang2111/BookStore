using System.Collections.Generic;

namespace BookStore.Models
{
    public class ShoppingCart
    {
        public List<Book> Books { get; set; }
        
        public ShoppingCart()
        {
            Books = new List<Book>();
        }
    }
}