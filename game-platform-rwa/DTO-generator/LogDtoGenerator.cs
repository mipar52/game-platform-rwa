using game_platform_rwa.DTOs;
using game_platform_rwa.Models;

namespace game_platform_rwa.DTO_generator
{
    public static class LogDtoGenerator
    {
        public static LogDto generateLogDto(LogEntry log)
        {
            return new LogDto
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Message = log.Message,
                Level = log.Level
            };
        }
    }
}
