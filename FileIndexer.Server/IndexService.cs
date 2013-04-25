using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace FileIndexer.Server
{
    internal class IndexService : ServiceBase
    {
        private async Task<int> Some()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            /*while (true)
            {
                
            }*/
            return 1;
        }
    }
}