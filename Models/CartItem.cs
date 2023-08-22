using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public required Cart Cart { get; set; }

    public int BookId { get; set; }

    public required Book Book { get; set; }

    public int Quantity { get; set; }
}


