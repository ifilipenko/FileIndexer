using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileIndexer.Index
{
    public enum ProcessingMode
    {
        Sequential,
        Parallel
    }

    public class IndexBuilder
    {
        private readonly ProcessingMode _processingMode;
        private readonly int _readBufferSize;

        public IndexBuilder(ProcessingMode processingMode, int readBufferSize = 4096)
        {
            _processingMode = processingMode;
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
                var tokens = MapTokens(binaryReader);

                var totalBytes = binaryReader.BaseStream.Length;
                ReduceTokensToIndex(tokens, totalBytes, lineIndex);
            }

            return lineIndex;
        }

        private IEnumerable<Token> MapTokens(BinaryReader binaryReader)
        {
            switch (_processingMode)
            {
                case ProcessingMode.Sequential:
                    return ReadPortion(binaryReader).SelectMany(ReadTokensFromBuffer).ToList();
                default:
                    return ReadPortion(binaryReader).AsParallel()
                                                    .SelectMany(ReadTokensFromBuffer)
                                                    .ToList();
            }
        }

        private static void ReduceTokensToIndex(IEnumerable<Token> tokens, long totalBytes, LineIndex lineIndex)
        {
            var line = new Line(0) {End = -1};
            var lastWordStart = 0L;
            foreach (var token in tokens.OrderBy(x => x.Range.Start))
            {
                if (lastWordStart < token.Range.Start)
                {
                    line.AddWord(lastWordStart, token.Range.Start - 1);
                }

                if (token.IsNewline)
                {
                    line.End = token.Range.Start - 1;
                    lineIndex.Add(line);
                    line = new Line(token.Range.End + 1) {End = -1};
                }
                lastWordStart = token.Range.End + 1;
            }

            if (line.Start > line.End && line.Start < totalBytes)
            {
                var lastPosition = totalBytes - 1;
                line.End = lastPosition;
                if (lastWordStart < lastPosition)
                {
                    line.AddWord(lastWordStart, lastPosition);
                }
                lineIndex.Add(line);
            }
        }

        private static IEnumerable<Token> ReadTokensFromBuffer(Buffer buffer)
        {
            var tokens = new List<Token>();
            Token whiteSpaceToken = null;
            for (int i = 0; i < buffer.Length; i++)
            {
                var position = buffer.GetPositionInStream(i);
                if (buffer[i] == '\r')
                {
                    var lineEnd = position;
                    var token = new Token {IsNewline = true};

                    if (i < buffer.Length - 1 && buffer[i + 1] == '\n')
                    {
                        token.Range = new Range(lineEnd, lineEnd + 1);
                        i++;
                    }
                    else
                    {
                        token.Range = new Range(lineEnd, lineEnd);
                    }
                    tokens.Add(token);
                }
                else
                {
                    if (char.IsWhiteSpace(buffer[i]))
                    {
                        if (whiteSpaceToken == null)
                        {
                            whiteSpaceToken = new Token
                                {
                                    IsWhitespace = true,
                                    Range = new Range(position, position)
                                };
                            tokens.Add(whiteSpaceToken);
                        }
                    }
                    else
                    {
                        if (whiteSpaceToken != null)
                        {
                            whiteSpaceToken.Range = new Range(whiteSpaceToken.Range.Start, position - 1);
                            whiteSpaceToken = null;
                        }
                    }
                }
            }

            if (whiteSpaceToken != null)
            {
                whiteSpaceToken.Range = new Range(whiteSpaceToken.Range.Start, buffer.LastBufferPosition);
                tokens.Add(whiteSpaceToken);
            }

            return tokens;
        }

        private IEnumerable<Buffer> ReadPortion(BinaryReader reader)
        {
            long bufferSize = _readBufferSize;
            var offset = 0L;
            do
            {
                bufferSize = Math.Min(bufferSize, reader.BaseStream.Length - offset);
                reader.BaseStream.Position = offset;
                var chars = reader.ReadChars((int) bufferSize);
                yield return new Buffer(chars, offset);
                offset += bufferSize;
            } while (offset < reader.BaseStream.Length);
        }

        class Token
        {
            public bool IsWhitespace
            {
                get { return !IsNewline; }
                set { IsNewline = !value; }
            }

            public bool IsNewline { get; set; }
            public Range Range { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: [{1}]", IsWhitespace ? "Spaces" : "Line end", Range);
            }
        }

        struct Buffer
        {
            private readonly char[] _chars;
            private readonly long _offset;
            
            public Buffer(char[] chars, long offset)
            {
                _chars = chars;
                _offset = offset;
            }

            public char this[int i]
            {
                get { return _chars[i]; }
            }

            public int Length
            {
                get { return _chars.Length; }
            }

            public long GetPositionInStream(int localIndex)
            {
                return localIndex + _offset;
            }

            public long LastBufferPosition
            {
                get { return Length - 1 + _offset; }
            }
        }
    }
}