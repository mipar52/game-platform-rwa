using game_platform_rwa.Models;

namespace game_platform_rwa.DTOs
{
    public class GameReviewDto
    {
        public int UserId { get; set; }

        public int GameId { get; set; }

        public int? Rating { get; set; }

        public string? ReviewText { get; set; }

        public bool? Approved { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Game Game { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
