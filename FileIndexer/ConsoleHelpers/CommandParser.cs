using System;
using System.Linq;

namespace FileIndexer.ConsoleHelpers
{
    public class CommandParser
    {
        private readonly ICommandFactory _commandFactory;

        public CommandParser(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public ICommand ParseCommandText(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                return null;

            if (commandText.StartsWith("get", StringComparison.OrdinalIgnoreCase))
            {
                var lines = commandText.Substring("get".Length)
                                       .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(x => Convert.ToInt32(x))
                                       .ToArray();
                if (lines.Length == 0)
                    throw new WrongCommandOrParametersException("\"get\" command has invalid format: line number is required");

                return _commandFactory.CreatePrintWordsCommand(lines[0], lines.Skip(1).ToArray());
            }

            if (commandText.StartsWith("exit", StringComparison.OrdinalIgnoreCase))
            {
                return _commandFactory.CreateExitCommand();
            }

            throw new WrongCommandOrParametersException("Unknown command or have invalid syntax");
        }
    }
}