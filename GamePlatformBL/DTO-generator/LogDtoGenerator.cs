using GamePlatformBL.DTOs;
using GamePlatformBL.Models;

namespace GamePlatformBL.DTO_generator
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
