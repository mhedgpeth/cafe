using Microsoft.Extensions.Logging;

namespace cafe
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory {get;} = new LoggerFactory().AddDebug(LogLevel.Debug);
        public static ILogger CreateLogger<T>() =>
            LoggerFactory.CreateLogger<T>();
    }
}