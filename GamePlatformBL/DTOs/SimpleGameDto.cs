using GamePlatformBL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformBL.DTOs
{
    public class SimpleGameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GenreName { get; set; }
        public string ImagePath { get; set; }
        public string ImageUrl { get; set; }
        public GameTypeDto GameType { get; set; }
        public List<GameGenreDto> GameGenres { get; set; } = new List<GameGenreDto>();
    }
}
