using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileIndexer
{
    public class IndexBuilder
    {
        private readonly int _readBufferSize;

        public IndexBuilder(int readBufferSize = 4096)
        {
            _readBufferSize = readBufferSize;
        }

        public LineIndex LoadFromCache(IIndexCache indexCache, string originalFilePath)
        {
            return indexCache.ActualCacheIsExists(originalFilePath) ? indexCache.LoadFromCache(originalFilePath) : null;
        }

        public LineIndex BuildFromStream(Stream stream, Encoding encoding)
        {
            var lineIndex = new LineIndex();

            using (var binaryReader = new BinaryReader(stream, encoding))
            {
                var lineStart = 0L;
                var lineEnd = -1L;
                var wordStart = -1L;

                var line = new Line(lineStart);
                foreach (var buffer in ReadPortion(binaryReader))
                {
                    for (int i = 0; i < buffer.Chars.Length; i++)
                    {
                        if (buffer.Chars[i] == '\r')
                        {
                            lineEnd = i + buffer.Offset;
                            line.End = Math.Max(0, lineEnd - 1);
                            if (wordStart >= 0)
                            {
                                line.AddWord(wordStart, line.Range.End);
                            }
                            lineIndex.Add(line);
                            if (i < buffer.Chars.Length - 1 && buffer.Chars[i + 1] == '\n')
                            {
                                i++;
                            }

                            lineStart = i + 1 + buffer.Offset;
                            line = new Line(lineStart);
                            wordStart = -1;
                        }
                        else
                        {
                            if (char.IsWhiteSpace(buffer.Chars[i]))
                            {
                                if (wordStart >= 0)
                                {
                                    line.AddWord(wordStart, i - 1 + buffer.Offset);
                                    wordStart = -1; 
                                }
                            }
                            else
                            {
                                if (wordStart < 0)
                                {
                                    wordStart = i + buffer.Offset;
                                }
                            }
                        }
                    }

                    if (lineStart > lineEnd && lineStart < binaryReader.BaseStream.Length && buffer.IsLast)
                    {
                        line.End = binaryReader.BaseStream.Length - 1;
                        if (wordStart >= 0)
                        {
                            line.AddWord(wordStart, line.Range.End);
                        }
                        lineIndex.Add(line);
                    }
                }
            }

            if (lineIndex.Lines.Count() == 1 && lineIndex.Lines.First().Range.IsEmpty)
                return new LineIndex();
            return lineIndex;
        }

        private IEnumerable<Buffer> ReadPortion(BinaryReader reader)
        {
            long bufferSize = _readBufferSize;
            var offset = 0L;
            do
            {
                bufferSize = Math.Min(bufferSize, reader.BaseStream.Length - offset);
                reader.BaseStream.Position = offset;
                yield return new Buffer(reader.ReadChars((int) bufferSize), offset, reader.BaseStream.Length);
                offset += bufferSize;
            } while (offset < reader.BaseStream.Length);
        }

        struct Buffer
        {
            private readonly char[] _chars;
            private readonly long _offset;
            private readonly long _totalBytes;

            public Buffer(char[] chars, long offset, long totalBytes)
            {
                _chars = chars;
                _offset = offset;
                _totalBytes = totalBytes;
            }

            public char[] Chars
            {
                get { return _chars; }
            }

            public long Offset
            {
                get { return _offset; }
            }

            public bool IsLast
            {
                get { return _totalBytes == _offset + _chars.Length; }
            }
        }
    }
}