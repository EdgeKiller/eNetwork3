using System;

namespace eNetwork3
{
    internal static class Logger
    {
        public static void Log(object message, int debugLevel)
        {
            if (debugLevel > 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[LOG] " + message);
                Console.ResetColor();
            }
        }

        public static void Error(object message, int debugLevel)
        {
            if (debugLevel > 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] " + message);
                Console.ResetColor();
            }
        }

        public static void Debug(object message, int debugLevel)
        {
            if (debugLevel > 2)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[DEBUG] " + message);
                Console.ResetColor();
            }
        }
    }
}
