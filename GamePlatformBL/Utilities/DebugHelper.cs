using System.Diagnostics;

namespace GamePlatformBL.Utilities
{
    public static class DebugHelper
    {
        public static void AppPrintDebugMessage(string text)
        {
            Debug.WriteLine($"[WebApp] - {text}");
        }
        public static void ApiPrintDebugMessage(string text)
        {
            Debug.WriteLine($"[WebAPI] - {text}");
        }
    }
}
