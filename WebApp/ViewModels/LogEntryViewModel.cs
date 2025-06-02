namespace WebApp.ViewModels
{
    public class LogEntryViewModel
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Level { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
