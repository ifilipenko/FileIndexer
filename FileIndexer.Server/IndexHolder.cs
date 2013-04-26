using System.Threading.Tasks;
using FileIndexer.Index;

namespace FileIndexer.Server
{
    internal class IndexHolder : IIndexHolder
    {
        public static readonly object SyncRoot = new object();
        public LineIndex Index { get; set; }

        public LineIndex GetIndex()
        {
            if (Index == null)
                throw new IndexIsNotAvailableException();
            return Index;
        }

        public void LoadIndexForFile(string filepath)
        {
            Task.Factory.StartNew(() =>
            {
                var index = IndexLoader.GetLineIndexForFile(filepath);
                lock (IndexHolder.SyncRoot)
                {
                    Index = index;
                }
            });
        }
    }
}