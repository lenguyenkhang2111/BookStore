using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class Book
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string Title { get; set; }

    [MaxLength(400)]
    public string? Description { get; set; }

    [DataType(DataType.Currency), Column(TypeName = "decimal(18,2)"), DisplayFormat(DataFormatString = "{0:C}")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
    public required decimal Price { get; set; }


    [Display(Name = "Image")]
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }


    [Display(Name = "Author Name")]
    public required string Author { get; set; }

    [Range(0, 100), Display(Name = "Discount Percentage")]
    public int DiscountPercentage { get; set; } = 0;

    [DataType(DataType.Date), Display(Name = "Year Published")]
    [DisplayFormat(DataFormatString = "{0:yyyy}", ApplyFormatInEditMode = true)]
    public required string YearPublished { get; set; }

    [Display(Name = "Final Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal FinalPrice => Price - Price * DiscountPercentage / 100;


}
