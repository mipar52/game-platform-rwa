using System;
using System.Collections.Generic;

namespace game_platform_rwa.Models;

public partial class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? GameUrl { get; set; }

    public int GameTypeId { get; set; }

    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

    public virtual GameType GameType { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
