using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{

    public class Register
    {
        public required string FullName { get; set; }

        public required string HomeAddress { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Not Match")]
        public required string ConfirmPassword { get; set; }
    }
    public class Login
    {
        [EmailAddress]
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public required string FullName { get; set; }
        public required string HomeAddress { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

    }

    public class EditProfileViewModel
    {
        [EmailAddress]
        public required string Email { get; set; }

        public required string FullName { get; set; }

        public required string HomeAddress { get; set; }
    }




    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public required string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public required string ConfirmNewPassword { get; set; }
    }




}

