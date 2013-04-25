using System.IO;
using Newtonsoft.Json;

namespace FileIndexer.Index
{
    public class LineIndexJsonFileCache : IIndexCache
    {
        private readonly string _cachesLocation;

        public LineIndexJsonFileCache(string cachesLocation)
        {
            _cachesLocation = cachesLocation;
        }

        public bool ActualCacheIsExists(string originalFilePath)
        {
            var cacheFile = new FileInfo(GetCacheFilePath(originalFilePath));
            var originalFile = new FileInfo(originalFilePath);
            return cacheFile.Exists && cacheFile.LastWriteTimeUtc > originalFile.LastWriteTimeUtc;
        }

        public LineIndex LoadFromCache(string originalFilePath)
        {
            var cacheFilePath = GetCacheFilePath(originalFilePath);
            var serializer = new JsonSerializer();
            using (var sw = new StreamReader(cacheFilePath))
            using (var reader = new JsonTextReader(sw))
            {
                return serializer.Deserialize<LineIndex>(reader);
            }
        }

        public void Update(LineIndex lineIndex, string originalFilePath)
        {
            var cacheFilePath = GetCacheFilePath(originalFilePath);
            var serializer = new JsonSerializer();
            using (var sw = new StreamWriter(cacheFilePath))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, lineIndex);
            }
        }

        private string GetCacheFilePath(string originalFilePath)
        {
            return Path.Combine(_cachesLocation, Path.GetFileName(originalFilePath) + ".cache");
        }
    }
}