using game_platform_rwa.Models;
using System.ComponentModel.DataAnnotations;

namespace game_platform_rwa.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? GameUrl { get; set; }
        public GameTypeDto GameType { get; set; }
        public int? MetacriticScore { get; set; }
        public bool WonGameOfTheYear { get; set; }
        public IEnumerable<GenreDto> Genres { get; set; } = new List<GenreDto>();
        public IEnumerable<GameReviewDto> Reviews { get; set; } = new List<GameReviewDto>();
    }
}
