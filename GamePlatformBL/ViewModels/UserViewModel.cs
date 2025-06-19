using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "The confirmed password should be at least 8 characters long and match with the password!")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = "";

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name should be between 2 and 50 characters long")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name should be between 2 and 50 characters long")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        [EmailAddress(ErrorMessage = "Provide a correct e-mail address")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = "";

        [Phone(ErrorMessage = "Provide a correct phone number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = "";

        public int RoleId { get; set; }
    }
}
