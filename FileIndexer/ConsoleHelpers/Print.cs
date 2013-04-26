using System;

namespace FileIndexer.ConsoleHelpers
{
    public static class Print
    {
        public static void PrintExceptionMessage(Exception exception)
        {
            PrintErrorMessage(exception.Message);
        }

        public static void PrintErrorMessage(string message)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}