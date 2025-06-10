namespace GamePlatformBL.ViewModels
{
    public class GameReviewViewModel
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public bool Approved { get; set; }
    }
}
