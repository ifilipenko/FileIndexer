using System.Threading.Tasks;
using FileIndexer.Index;

namespace FileIndexer.Server
{
    internal class IndexHolder : IIndexHolder
    {
        private static readonly object _syncRoot = new object();
        private LineIndex _index;

        public LineIndex GetIndex()
        {
            if (_index == null)
                throw new IndexIsNotAvailableException();
            return _index;
        }

        public void LoadIndexForFile(string filepath)
        {
            Task.Factory.StartNew(() =>
            {
                var index = IndexLoader.GetLineIndexForFile(filepath);
                lock (_syncRoot)
                {
                    _index = index;
                }
            });
        }
    }
}