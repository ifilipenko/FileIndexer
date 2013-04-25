using System.ServiceProcess;

namespace FileIndexer.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase.Run(new IndexService());
        }
    }
}