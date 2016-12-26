using System;
using Microsoft.Extensions.Logging;

namespace cafe.CommandLine
{
    public sealed class Presenter
    {
        public static void ShowApplicationHeading(ILogger logger, params string[] args)
        {
            var message = $"cafe {System.Reflection.Assembly.GetEntryAssembly().GetName().Version} with arguments {string.Join(" ", args)}";
            logger.LogInformation(message);
            WriteMessage(message, ConsoleColor.DarkYellow);
            Console.Out.WriteLine(); // empty line for readability
        }

        private static void WriteMessage(string message, ConsoleColor foregroundColor)
        {
            var colorBefore = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.Out.WriteLine(message);
            Console.ForegroundColor = colorBefore;
        }

        public static void ShowMessage(string message, ILogger logger)
        {
            logger.LogInformation(message);
            WriteMessage(message, Console.ForegroundColor);
        }

        public static void ShowError(string message, ILogger logger)
        {
            logger.LogError(message);
            WriteMessage(message, ConsoleColor.DarkRed);
        }
    }
}