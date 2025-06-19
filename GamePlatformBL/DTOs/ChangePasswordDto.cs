using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.DTOs
{
    public class ChangePasswordDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "The new password should be at least 8 characters long!")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string NewPassword { get; set; } = null!;
    }

}
