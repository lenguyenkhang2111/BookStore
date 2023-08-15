using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }
}
