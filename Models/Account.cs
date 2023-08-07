using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{

        public class Register
        {
            [Required]
            public string FullName { get; set; }

            [Required]
            public string HomeAddress { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Not Match")]
            public string ConfirmPassword { get; set; }
        }
            public class Login
            {
                [Required]
                [EmailAddress]
                public string Email { get; set; }

                [Required]
                [DataType(DataType.Password)]
                public string Password { get; set; }

                [Display(Name = "Remember me")]
                public bool RememberMe { get; set; }
            }

            public class ProfileViewModel
            {
                public string FullName { get; set; }
                public string HomeAddress { get; set; }
                    
                [EmailAddress]
                public string Email { get; set; }

            }

            public class EditProfileViewModel
            {
                [Required]
                [EmailAddress]
                public string Email { get; set; }

                [Required]
                public string FullName { get; set; }

                [Required]
                public string HomeAddress { get; set; }
            }




    public class ChangePasswordViewModel
                {
                [Required]
                [DataType(DataType.Password)]
                [Display(Name = "Current Password")]
                public string CurrentPassword { get; set; }

                [Required]
                [DataType(DataType.Password)]
                [Display(Name = "New Password")]
                public string NewPassword { get; set; }

                [DataType(DataType.Password)]
                [Display(Name = "Confirm New Password")]
                [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
                public string ConfirmNewPassword { get; set; }
                }

                


}

