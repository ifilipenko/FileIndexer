using FileIndexer.Console;
using FileIndexer.Index;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FileIndexer.Tests.ConsoleTests
{
    [TestFixture]
    public class PrintLineWordsTests
    {
        private LineIndex _index;
        private IStringsSource _stringsSource;

        [SetUp]
        public void SetUp()
        {
            _index = new LineIndex();

            var line1 = new Line(1, 12);
            line1.AddWord(1, 5);
            line1.AddWord(6, 10);
            _index.Add(line1);

            var line2 = new Line(15, 25);
            line2.AddWord(16, 17);
            line2.AddWord(18, 20);
            line2.AddWord(21, 23);
            _index.Add(line2);

            _stringsSource = Substitute.For<IStringsSource>();
        }

        [Test]
        public void Ctor_should_initialize_line_index_and_words_array()
        {
            var command = new PrintLineWords(1, new[] {1, 2, 3}, _index, _stringsSource);

            command.Line.Should().Be(1);
            command.WordIndexes.Should().BeEquivalentTo(new[] {1, 2, 3});
        }

        [Test]
        public void Should_load_line_from_string_source()
        {
            var command = new PrintLineWords(1, new int[0], _index, _stringsSource);

            command.Execute();

            _stringsSource.Received(1).ReadString(15, 25);
        }

        [Test]
        public void Should_limit_loading_line_content_with_one_kilobyte()
        {
            _index.Add(new Line(27, 2000));
            var command = new PrintLineWords(2, new int[0], _index, _stringsSource);

            command.Execute();

            _stringsSource.Received(1).ReadString(27, Volumes.Kilobyte - 1 + 27);
        }

        [Test]
        public void Should_load_words_of_specified_line()
        {
            var command = new PrintLineWords(1, new[] {1, 2}, _index, _stringsSource);

            command.Execute();

            _stringsSource.Received(1).ReadString(18, 20);
            _stringsSource.Received(1).ReadString(21, 23);

        }
    }
}