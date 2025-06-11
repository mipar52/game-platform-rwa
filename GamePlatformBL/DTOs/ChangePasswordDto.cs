namespace GamePlatformBL.DTOs
{
    public class ChangePasswordDto
    {
        public int Id { get; set; }
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

}
