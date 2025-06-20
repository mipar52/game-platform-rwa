﻿namespace GamePlatformBL.ViewModels
{
    public class GameDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GameUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int? MetacriticScore { get; set; }
        public bool WonGameOfTheYear { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public GameTypeViewModel GameType { get; set; }
        public List<GameGenreViewModel> GameGenres { get; set; }
        public List<GameReviewViewModel> Reviews { get; set; }
    }
}
