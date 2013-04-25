using System;

namespace FileIndexer
{
    public class ExitCommand : ICommand
    {
        public void Execute(LineIndex index, IStringsSource stringSource)
        {
            Environment.Exit(0);
        }
    }
}