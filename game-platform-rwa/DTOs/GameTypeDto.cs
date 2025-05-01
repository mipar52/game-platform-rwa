using game_platform_rwa.Models;

namespace game_platform_rwa.DTOs
{
    public class GameTypeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
