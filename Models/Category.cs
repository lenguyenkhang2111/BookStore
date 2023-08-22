namespace BookStore.Models;

public class Category
{
    public int Id { get; set; }

    public required string CategoryName { get; set; }

    public ICollection<Book>? Books { get; set; }


}

