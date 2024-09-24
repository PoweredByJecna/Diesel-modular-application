using System.ComponentModel.DataAnnotations;

namespace Diesel_modular_application.Models
{
    public class RegisterMod
    {

        public required InputModel Input { get; set; }

        public class InputModel 
        {
            [Required]
            [Display(Name = "UserName")]
            public required string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public required string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public required string ConfirmPassword { get; set; }
        }
    }
}
