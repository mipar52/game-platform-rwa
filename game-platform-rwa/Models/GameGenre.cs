using System;
using System.Collections.Generic;

namespace game_platform_rwa.Models;

public partial class GameGenre
{
    public int GameId { get; set; }

    public int GenreId { get; set; }

    public DateTime? AddedOn { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;
}
