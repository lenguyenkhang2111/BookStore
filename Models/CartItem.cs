using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class CartItem
    {
        public int Id { get; set; }

        public Cart Cart { get; set; }
        public CartItem(Cart cart)
        {
            Cart = cart;
        }
        public CartItem()
        {
        }

        public Book Book { get; set; }

        public int Quantity { get; set; }
    }
