using System.Diagnostics;

namespace WebApp.Utilities
{
    public static class DebugHelper
    {
        public static void PrintDebugMessage(string text)
        {
            Debug.WriteLine($"[WebApp] - {text}");
        }
    }
}
