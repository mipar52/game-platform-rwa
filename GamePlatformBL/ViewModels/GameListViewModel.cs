namespace GamePlatformBL.ViewModels
{
    public class GameListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GenreName { get; set; }
        public string ImagePath { get; set; }
        public string ImageUrl { get; set; }
        public GameTypeViewModel GameType { get; set; }
    }

}
