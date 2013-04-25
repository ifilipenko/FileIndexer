using FileIndexer.Index;
using FluentAssertions;
using NUnit.Framework;
using Newtonsoft.Json;

namespace FileIndexer.Tests.IndexTests
{
    [TestFixture]
    public class LineIndexTests
    {
        [Test]
        public void LineIndex_should_add_line()
        {
            var index = new LineIndex();

            index.Add(new Line(1, 10));

            index.Lines.Should().HaveCount(1);
        }

        [Test]
        public void LineIndex_should_return_line_range()
        {
            var index = new LineIndex();
            index.Add(new Line(1, 10));
            index.Add(new Line(10, 20));
            index.Add(new Line(20, 25));

            var lineRange = index.GetLineRange(1);

            lineRange.Start.Should().Be(10);
            lineRange.End.Should().Be(20);
        }

        [Test]
        public void LineIndex_should_return_word_range_from_specified_line()
        {
            var index = new LineIndex();

            var line1 = new Line(1, 12);
            line1.AddWord(1, 5);
            line1.AddWord(6, 10);
            index.Add(line1);

            var line2 = new Line(15, 25);
            line2.AddWord(16, 17);
            line2.AddWord(18, 20);
            line2.AddWord(21, 23);
            index.Add(line2);

            var wordRange = index.GetWordRange(1, 1);

            wordRange.Start.Should().Be(18);
            wordRange.End.Should().Be(20);
        }

        [Test]
        public void LineIndex_should_be_can_be_stored_in_JSON_format()
        {
            var index = new LineIndex();

            var line1 = new Line(1, 12);
            line1.AddWord(1, 5);
            line1.AddWord(6, 10);
            index.Add(line1);

            var line2 = new Line(15, 25);
            line2.AddWord(16, 17);
            line2.AddWord(18, 20);
            line2.AddWord(21, 23);
            index.Add(line2);

            var json = JsonConvert.SerializeObject(index, Formatting.Indented);
            var deserializedLineIndex = JsonConvert.DeserializeObject<LineIndex>(json);

            deserializedLineIndex.Should().Be(index);
        }
    }
}