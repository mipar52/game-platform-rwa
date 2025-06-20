﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GamePlatformBL.DTOs
{
    public class GameCreateDto
    {
        [Required(ErrorMessage = "You need to enter the name of the game!")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        
        public DateOnly? ReleaseDate { get; set; }

        [Required(ErrorMessage = "You need to enter the game URL!")]
        public string? GameUrl { get; set; }

        [Required(ErrorMessage = "You need to enter the game type!")]
        public int GameTypeId { get; set; }

        [Required(ErrorMessage = "You need to enter at least one genre!")]
        public List<int> GenreIds { get; set; } = new();

        public bool WonGameOfTheYear { get; set; } = false;

        public int MetaCriticScore { get; set; } = 0;
        public string? ImagePath { get; set; }
        public string? ImageUrl { get; set; }

    }

}
