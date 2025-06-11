namespace GamePlatformBL.ViewModels
{
    public class AdminGameReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public int Rating { get; set; }
        public bool Approved { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserViewModel? User { get; set; }
        public GameViewModel? Game { get; set; }
    }
}
