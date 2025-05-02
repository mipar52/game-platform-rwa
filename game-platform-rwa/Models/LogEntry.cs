using System;
using System.Collections.Generic;

namespace game_platform_rwa.Models;

public partial class LogEntry
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;
}
