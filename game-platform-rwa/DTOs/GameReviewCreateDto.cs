namespace game_platform_rwa.DTOs
{
    public class GameReviewCreateDto
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int? Rating { get; set; }
        public string? ReviewText { get; set; }
        public bool? Approved { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
