using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class Book
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    [DataType(DataType.Currency), Column(TypeName = "decimal(18,2)")]
    public required decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public Category? Category { get; set; }

    public required string Author { get; set; }

    [Range(0, 100)]
    public int DiscountPercentage { get; set; } = 0;

    [DataType(DataType.Date), Display(Name = "Date Published")]
    [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
    public required string DatePublished { get; set; }

    [Display(Name = "Final Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]

    public decimal FinalPrice => Price - Price * DiscountPercentage / 100;

}
