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

        private static void Log(string message, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = consoleColor;

        }
    }
}
