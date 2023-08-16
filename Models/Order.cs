namespace BookStore.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public required string UserId { get; set; }

    public required List<OrderItem> OrderItems { get; set; }
}
