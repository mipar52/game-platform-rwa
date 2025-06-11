namespace GamePlatformBL.ViewModels
{
    public class GameTypeGenreSelectionViewModel
    {
        public int? SelectedGameTypeId { get; set; }
        public List<GameTypeViewModel> GameTypes { get; set; }

        public List<int> SelectedGenreIds { get; set; } = new();
        public List<GenreViewModel> Genres { get; set; }
    }
}
