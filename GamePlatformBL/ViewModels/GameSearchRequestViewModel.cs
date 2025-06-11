namespace GamePlatformBL.ViewModels
{
    public class GameSearchRequestViewModel
    {
        public int GameTypeId { get; set; }
        public List<int> SelectedGenreIds { get; set; } = new();
    }
}
