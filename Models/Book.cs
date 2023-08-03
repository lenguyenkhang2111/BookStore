using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class Book
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public Category? Category { get; set; }

}
