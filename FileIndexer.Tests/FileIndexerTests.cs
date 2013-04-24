using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FileIndexer.Tests
{
    [TestFixture]
    public class IndexBuilderTests
    {
        private const int ReadBufferSize = 10;
        private IndexBuilder _indexBuilder;

        [SetUp]
        public void SetUp()
        {
            _indexBuilder = new IndexBuilder(ReadBufferSize);
        }

        [Test]
        public void BuildFromStream_should_build_index_with_empty_lines_from_empty_file()
        {
            const string text = "";
            using (var stream = CreateStreamFromText(text))
            {
                var index = _indexBuilder.BuildFromStream(stream, IndexBuilder.DefaultEncoding);

                index.Lines.Should().BeEmpty();
            }
        }

        [Test]
        public void BuildFromStream_should_build_index_with_one_line_without_line_endings()
        {
            const string text = "sdfhsjdkfhsjkdfhskdjh fksjdfhks djfhksdfh sd";
            using (var stream = CreateStreamFromText(text))
            {
                var index = _indexBuilder.BuildFromStream(stream, IndexBuilder.DefaultEncoding);

                index.Lines.Should().HaveCount(1);
                index.Lines.First().Range.Length.Should().Be(text.Length);
            }
        }

        [TestCase("sdfhsjdkfhsjkdfhskdjh fksjdfhks djfhksdfh sd\r", Description = "Unix-style line endings")]
        [TestCase("sdfhsjdkfhsjkdfhskdjh fksjdfhks djfhksdfh sd\r\n", Description = "Windows-style line endings")]
        public void BuildFromStream_should_build_index_with_one_line_with_line_endings(string text)
        {
            using (var stream = CreateStreamFromText(text))
            {
                var expectedLineLength = text.TrimEnd(Environment.NewLine.ToCharArray()).Length;
                
                var index = _indexBuilder.BuildFromStream(stream, IndexBuilder.DefaultEncoding);

                index.Lines.Should().HaveCount(1);
                index.Lines.First().Range.Length.Should().Be(expectedLineLength);
            }
        }

        [TestCase("\r", Description = "Unix-style line endings")]
        [TestCase("\r\n", Description = "Windows-style line endings")]
        public void BuildFromStream_should_build_index_for_multy_line_text(string lineEndings)
        {
            var lines = new[]
                {
                    new string('w', ReadBufferSize - lineEndings.Length),
                    new string('w', 2*ReadBufferSize - lineEndings.Length),
                    new string('w', 3*ReadBufferSize - lineEndings.Length),
                    new string('w', (int) (1.5*ReadBufferSize - lineEndings.Length))
                };
            var text = string.Join(lineEndings, lines);

            using (var stream = CreateStreamFromText(text))
            {
                var index = _indexBuilder.BuildFromStream(stream, IndexBuilder.DefaultEncoding);

                index.Lines.Should().HaveSameCount(lines);
                for (int i = 0; i < lines.Length; i++)
                {
                    index.Lines.ElementAt(i).Range.Length.Should().Be(lines[i].Length, "Line #{0} from index have unexpected length", i);
                }
            }
        }

        [TestCase("\r", Description = "Unix-style line endings")]
        [TestCase("\r\n", Description = "Windows-style line endings")]
        public void BuildFromStream_should_build_index_with_words_bounds(string lineEndings)
        {
            var lines = new[]
                {
                    "www ddd fff",
                    "ddddd\tkkkkkkkk  yyyyyyy  xxx   ",
                    "   dd\t kkk  yyyy  xyx  444444"
                };
            var text = string.Join(lineEndings, lines);

            using (var stream = CreateStreamFromText(text))
            {
                var index = _indexBuilder.BuildFromStream(stream, IndexBuilder.DefaultEncoding);

                var line1 = index.Lines.ElementAt(0);
                var line2 = index.Lines.ElementAt(1);
                var line3 = index.Lines.ElementAt(2);

                line1.Words.Should().HaveCount(3);
                line2.Words.Should().HaveCount(4);
                line3.Words.Should().HaveCount(5);
            }
        }

        [Test]
        public void LoadFromCache_should_load_index_from_cache()
        {
            var cachedIndex = new LineIndex();
            var indexCache = Substitute.For<IIndexCache>();
            indexCache.ActualCacheIsExists(Arg.Any<string>()).Returns(true);
            indexCache.LoadFromCache(Arg.Any<string>()).Returns(cachedIndex);

            var index = _indexBuilder.LoadFromCache(indexCache, "original");

            index.Should().BeSameAs(cachedIndex);
        }

        [Test]
        public void LoadFromCache_should_return_null_when_cache_is_not_actual()
        {
            var indexCache = Substitute.For<IIndexCache>();
            indexCache.ActualCacheIsExists(Arg.Any<string>()).Returns(false);

            var index = _indexBuilder.LoadFromCache(indexCache, "original");

            index.Should().BeNull();
            indexCache.DidNotReceiveWithAnyArgs().LoadFromCache(null);
        }

        private static MemoryStream CreateStreamFromText(string text)
        {
            return new MemoryStream(IndexBuilder.DefaultEncoding.GetBytes(text));
        }
    }
}