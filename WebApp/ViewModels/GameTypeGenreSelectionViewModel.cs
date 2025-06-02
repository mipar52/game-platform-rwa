namespace WebApp.ViewModels
{
    public class GameTypeGenreSelectionViewModel
    {
        public int? SelectedGameTypeId { get; set; }
        public List<GameTypeViewModel> GameTypes { get; set; }

        public List<int> SelectedGenreIds { get; set; } = new();
        public List<GameGenreViewModel> GameGenres { get; set; }
    }
}
