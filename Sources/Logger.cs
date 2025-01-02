using System;

namespace WG2
{
    public static class Logger
    {
        public static void LogMessage(string message)
        {
            Log(message, ConsoleColor.White);
        }

        public static void LogError(string message)
        {
            Log($"Error: {message}", ConsoleColor.Red);
        }

        public static void LogDebug(string message)
        {
            Log($"Debug: {message}", ConsoleColor.Magenta);
        }

        private static void Log(string message, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = consoleColor;

        }
    }
}
