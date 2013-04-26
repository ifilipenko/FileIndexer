using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using FileIndexer.Index;

namespace FileIndexer.Server
{
    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class IndexService
    {
        private const int MaxOutputLength = 1024;
        private readonly IStringsSource _stringSource;
        private readonly IIndexHolder _indexHolder;

        public IndexService(IStringsSource stringSource, IIndexHolder indexHolder)
        {
            _stringSource = stringSource;
            _indexHolder = indexHolder;
        }

        [OperationContract]
        public string Get(int lineIndex, int[] words)
        {
            try
            {
                var index = _indexHolder.GetIndex();
                if (words.Length == 0)
                {
                    var lineRange = index.GetLineRange(lineIndex);
                    return ReadTextToString(lineRange);
                }

                return string.Join(" ", ReadWords(lineIndex, words));
            }
            catch (LineNotFoundException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (WordNotFoundException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (IndexIsNotAvailableException ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        private IEnumerable<string> ReadWords(int lineIndex, IEnumerable<int> words)
        {
            return words.Select(word => _indexHolder.GetIndex().GetWordRange(lineIndex, word))
                        .Select(ReadTextToString);
        }

        private string ReadTextToString(Range lineRange)
        {
            var suffix = string.Empty;
            if (lineRange.Length > MaxOutputLength)
            {
                lineRange = lineRange.Truncate(MaxOutputLength);
                suffix = "...";
            }
            var @string = lineRange.IsEmpty ? string.Empty : _stringSource.ReadString(lineRange.Start, lineRange.End);
            return @string + suffix;
        }

        [OperationContract(Name = "GetAsync", AsyncPattern = true/*, Action = "http://tempuri.org/GetAsync", ReplyAction = "http://tempuri.org/GetAsync"*/)]
        public async Task<string> GetAsync(int lineIndex, int[] words)
        {
            return Get(lineIndex, words);
        }
    }
}