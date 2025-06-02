namespace WebApp.ViewModels
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public GameTypeViewModel GameType { get; set; } 
        public List<GameGenreViewModel> Genres { get; set; }
    }

}
