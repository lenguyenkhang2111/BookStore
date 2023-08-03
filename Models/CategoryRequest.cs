using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class CategoryRequest
{
    public int Id { get; set; }

    public required string CategoryName { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.Now;

    public required User StoreOwner { get; set; }



    public bool IsApproved { get; set; }

}
