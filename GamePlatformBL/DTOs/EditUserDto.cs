using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.DTOs
{
    public class EditUserDto
    {
        [Required(ErrorMessage = "User name is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Provide a correct e-mail address")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name should be between 2 and 50 characters long")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name should be between 2 and 50 characters long")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Provide a correct phone number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
    }
}
