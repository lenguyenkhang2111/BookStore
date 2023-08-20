using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using System.ComponentModel.DataAnnotations;

namespace BookStore;

public class EditViewModel
{
    public string UserId { get; set; }

    public string? RoleName { get; set; }

    public string? changePassword { get; set; }
}

public class CreateByAdmin
{

    [EmailAddress]
    public required string Email { get; set; }
    public string? FullName { get; set; }

    public string? HomeAddress { get; set; }

    [DataType(DataType.Password)]
    public required string? Password { get; set; }
}
