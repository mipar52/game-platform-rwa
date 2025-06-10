using GamePlatformBL.Models;

namespace GamePlatformBL.ViewModels
{
    public class AdminGameViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? GameUrl { get; set; }
        public GameTypeViewModel GameType { get; set; }

        public int GameTypeId { get; set; }
        public int? MetacriticScore { get; set; }
        public bool WonGameOfTheYear { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public IEnumerable<GameGenreViewModel> GameGenres { get; set; } = new List<GameGenreViewModel>();
        public IEnumerable<int> genreIds { get; set; } = new List<int>();
        public IEnumerable<GameReviewViewModel> Reviews { get; set; } = new List<GameReviewViewModel>();
    }
}
