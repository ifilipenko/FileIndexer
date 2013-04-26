using System;

namespace FileIndexer.ConsoleHelpers
{
    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Environment.Exit(0);
        }
    }
}