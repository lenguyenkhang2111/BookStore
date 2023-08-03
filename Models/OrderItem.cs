namespace BookStore.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public required Book Book { get; set; }

    public required Order Order { get; set; }
}
