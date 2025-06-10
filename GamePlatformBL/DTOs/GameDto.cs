using GamePlatformBL.Models;
using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.DTOs
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
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public IEnumerable<GameGenreDto> GameGenres { get; set; } = new List<GameGenreDto>();
        public IEnumerable<GameReviewDto> Reviews { get; set; } = new List<GameReviewDto>();
    }
}
