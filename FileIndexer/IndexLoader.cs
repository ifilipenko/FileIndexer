using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FileIndexer.Index;

namespace FileIndexer
{
    public static class IndexLoader
    {
        public static LineIndex GetLineIndexForFile(string filePath)
        {
            var indexCache = new LineIndexJsonFileCache(AppDomain.CurrentDomain.BaseDirectory);
            var indexBuilder = new IndexBuilder(ProcessingMode.Parallel, 4 * Volumes.Kilobyte);

            Console.WriteLine("Checking index cache...");
            var lineIndex = indexBuilder.LoadFromCache(indexCache, filePath);
            if (lineIndex == null)
            {
                Console.WriteLine("Index cache not found or out of date. Start indexing...");

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var stream = File.OpenRead(filePath))
                {
                    lineIndex = indexBuilder.BuildFromStream(stream, FixedParameters.Encoding);
                }
                stopwatch.Stop();

                Console.WriteLine("Indexing complete at {0}.", stopwatch.Elapsed);
                indexCache.Update(lineIndex, filePath);
            }
            else
            {
                Console.WriteLine("Index loaded from cache.");
            }
            Console.WriteLine("File consist of {0} lines", lineIndex.Lines.Count());
            return lineIndex;
        }
    }
}