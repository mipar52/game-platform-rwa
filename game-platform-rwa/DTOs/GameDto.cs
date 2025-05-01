using game_platform_rwa.Models;
using System.ComponentModel.DataAnnotations;

namespace game_platform_rwa.DTOs
{
    public class GameDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You need to enter the name of the game!")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateOnly? ReleaseDate { get; set; }
        
        [Required(ErrorMessage = "You need to enter the game URL!")]
        public string? GameUrl { get; set; }

        public int GameTypeId { get; set; }
        
        [Required(ErrorMessage = "You need to enter the game type!")]
        public virtual GameType GameType { get; set; } = null!;

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        
        [Required(ErrorMessage = "You need to enter the game genre!")]
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
