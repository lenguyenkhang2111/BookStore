using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;
public class User : IdentityUser
{
    public required string FullName { get; set; }
    public required string HomeAddress { get; set; }
    public string? ImageUrl { get; set; }

}

