using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;
public class User : IdentityUser
{
    public string FullName { get; set; }
    public string HomeAddress { get; set; }

    public string Email { get; set; }   
    public string? ImageUrl { get; set; }
}