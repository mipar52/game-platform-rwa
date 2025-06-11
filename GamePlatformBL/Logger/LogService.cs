using GamePlatformBL.Models;

namespace GamePlatformBL.Logger
{
    public class LogService
    {
        private readonly GamePlatformRwaContext _context;

        public LogService(GamePlatformRwaContext context)
        {
            _context = context;
        }

        public void Log(string message, string level = "Info")
        {
            var log = new LogEntry
            {
                Message = message,
                Level = level,
                Timestamp = DateTime.UtcNow
            };

            _context.LogEntries.Add(log);
            _context.SaveChanges();
        }
    }
}
