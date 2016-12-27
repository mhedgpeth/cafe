using System;
using NLog;

namespace cafe.CommandLine
{
    public sealed class Presenter
    {
        public static void ShowApplicationHeading(Logger logger, params string[] args)
        {
            var message = $"cafe {System.Reflection.Assembly.GetEntryAssembly().GetName().Version} with arguments {string.Join(" ", args)}";
            logger.Info(message);
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

        public static void ShowMessage(string message, Logger logger)
        {
            logger.Info(message);
            WriteMessage(message, Console.ForegroundColor);
        }

        public static void ShowError(string message, Logger logger)
        {
            logger.Error(message);
            WriteMessage(message, ConsoleColor.DarkRed);
        }

        public static void NewLine()
        {
            Console.Out.WriteLine();
        }
    }
}