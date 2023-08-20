using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class CategoryRequest
{
    public int Id { get; set; }

    public required string CategoryName { get; set; }

    public required string Description { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.Now;


    public string? StoreOwnerId { get; set; }


    public string Status { get; set; } = "Pending";

}
