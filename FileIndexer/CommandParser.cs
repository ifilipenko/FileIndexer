using System;
using System.Linq;

namespace FileIndexer
{
    public class CommandParser
    {
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

                return new PrintLineWords(lines[0], lines.Skip(1).ToArray());
            }

            if (commandText.StartsWith("exit", StringComparison.OrdinalIgnoreCase))
            {
                return new ExitCommand();
            }

            throw new WrongCommandOrParametersException("Unknown command or have invalid syntax");
        }
    }
}