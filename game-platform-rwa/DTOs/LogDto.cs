namespace game_platform_rwa.DTOs
{
    public class LogDto
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Level { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
