using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class CartItem
{
    public int Id { get; set; }

    public required Cart Cart { get; set; }

    public required Book Book { get; set; }

    public int Quantity { get; set; }
}
