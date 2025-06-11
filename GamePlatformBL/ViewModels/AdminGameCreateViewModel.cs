using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.ViewModels
{
    public class AdminGameCreateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required]
        public int GameTypeId { get; set; }

        public List<int> GenreIds { get; set; } = new();

        [Range(0, 100)]
        public int? MetacriticScore { get; set; }

        public bool WonGameOfTheYear { get; set; }

        [Url]
        public string? GameUrl { get; set; }

        [Url]
        public string? ImageUrl { get; set; }
    }
}
