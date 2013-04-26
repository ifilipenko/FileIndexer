using System;
using FileIndexer.ConsoleHelpers;

namespace FileIndexer.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = ParseParameters(args);

            try
            {
                Console.WriteLine("Type command: ");
                var commandParser = new CommandParser(new ServiceClientCommandFactory(parameters.ServerAddress));
                
                while (true)
                {
                    try
                    {
                        var commandText = Console.ReadLine();
                        var command = commandParser.ParseCommandText(commandText);
                        if (command != null)
                        {
                            command.Execute();
                        }
                    }
                    catch (WrongCommandOrParametersException ex)
                    {
                        PrintExceptionMessage(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                PrintExceptionMessage(ex);
            }
        }

        private static Parameters ParseParameters(string[] args)
        {
            if (args.Length == 1)
            {
                var address = args[0];
                if (string.IsNullOrWhiteSpace(address))
                    throw new ArgumentException("Address is required", "args");

                return new Parameters {ServerAddress = address};
            }
            throw new ArgumentException("Expected 1 parameter: server address", "args");
        }

        private static void PrintExceptionMessage(Exception exception)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
