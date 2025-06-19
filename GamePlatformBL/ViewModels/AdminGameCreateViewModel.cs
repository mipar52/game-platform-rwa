using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.ViewModels
{
    public class AdminGameCreateViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Game name is required")]
        [Display(Name = "Game Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Game description is required")]
        [Display(Name = "Game Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Game release date is required")]
        [Display(Name = "Game inital release date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Game type is required!")]
        [Display(Name = "Game type")]
        public int GameTypeId { get; set; }

        public List<int> GenreIds { get; set; } = new();

        [Range(0, 100)]
        public int? MetacriticScore { get; set; }

        public bool WonGameOfTheYear { get; set; }

        [Required(ErrorMessage = "Game URL date is required")]
        [Display(Name = "Official Game URL")]
        [Url]
        public string? GameUrl { get; set; }

        [Url]
        public string? ImageUrl { get; set; }
    }
}
