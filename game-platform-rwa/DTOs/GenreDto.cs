using game_platform_rwa.Models;

namespace game_platform_rwa.DTOs
{
    public class GenreDto
    {
        public string Name { get; set; } = null!;

        public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    }
}
