namespace BookStore.Models;

public class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public required User User { get; set; }

    public required List<OrderItem> OrderItems { get; set; }
}
