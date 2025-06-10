namespace GamePlatformBL.ViewModels
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImagePath { get; set; }
        public GameTypeViewModel GameType { get; set; } 
        public List<GameGenreViewModel> GameGenres { get; set; }
    }

}
