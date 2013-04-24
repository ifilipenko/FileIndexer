using System.Diagnostics;

namespace FileIndexer
{
    public class ExitCommand : ICommand
    {
        public void Execute(LineIndex index, IStringsSource stringSource)
        {
            Process.GetCurrentProcess().Close();
        }
    }
}