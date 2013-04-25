using System;
using FileIndexer.Index;

namespace FileIndexer.Console
{
    public class ExitCommand : ICommand
    {
        public void Execute(LineIndex index, IStringsSource stringSource)
        {
            Environment.Exit(0);
        }
    }
}