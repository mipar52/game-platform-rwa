namespace WebApp.ViewModels
{
    public class UserReviewViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string GameName { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

