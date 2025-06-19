using GamePlatformBL.Models;
using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.ViewModels
{
    public class AdminGameViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Game name is required")]
        [Display(Name = "Game Name")]
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Game description is required")]
        [Display(Name = "Game Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Game release date is required")]
        [Display(Name = "Game inital release date")]
        public DateOnly? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Game URL date is required")]
        [Display(Name = "Official Game URL")]
        public string? GameUrl { get; set; }

        [Required(ErrorMessage = "Game type is required!")]
        [Display(Name = "Game type")]
        public GameTypeViewModel GameType { get; set; }

        public int GameTypeId { get; set; }

        [Required(ErrorMessage = "Game type is required!")]
        [Display(Name = "Official Metacritic Score")]
        public int? MetacriticScore { get; set; }

        [Display(Name = "Won Game Of the Year Award (GOY)")]
        public bool WonGameOfTheYear { get; set; }

        [Display(Name = "Game image")]
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public IEnumerable<GameGenreViewModel> GameGenres { get; set; } = new List<GameGenreViewModel>();
        public IEnumerable<int> genreIds { get; set; } = new List<int>();
        public IEnumerable<GameReviewViewModel> Reviews { get; set; } = new List<GameReviewViewModel>();
    }
}
