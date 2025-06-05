using game_platform_rwa.Models;

namespace game_platform_rwa.DTOs
{
    public class GameReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int GameId { get; set; }

        public int? Rating { get; set; }

        public string? ReviewText { get; set; }

        public bool? Approved { get; set; }

        public DateTime? CreatedAt { get; set; }

        public User? User { get; set; }
        public Game? Game { get; set; }

    }
}
