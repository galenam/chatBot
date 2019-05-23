using System;
using Microsoft.Extensions.Logging;

namespace BotConsole.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogException<T>(this ILogger<T> logger, Exception ex)
        {
            logger.LogError(ex, $"ex.InnerException = {ex.InnerException}, ex.StackTrace = {ex.StackTrace}");
        }
    }
}