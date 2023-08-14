namespace BookStore.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int BookId { get; set; }
    public virtual Book Book { get; set; }

    public int OrderID { get; set; }
    public virtual Order Order { get; set; }
}
