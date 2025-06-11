namespace GamePlatformBL.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
    }
}
