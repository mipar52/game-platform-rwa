﻿using System;
using System.Collections.Generic;

namespace GamePlatformBL.Models;

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}
