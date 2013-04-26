using System;
using System.ServiceModel;
using FileIndexer.Index;
using FileIndexer.Server;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace FileIndexer.Tests.ServiceTests
{
    [TestFixture]
    public class IndexServiceTests
    {
        private LineIndex _index;
        private IStringsSource _stringsSource;
        private IndexService _indexService;
        private IIndexHolder _indexHolder;

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
            _indexHolder = Substitute.For<IIndexHolder>();
            _indexHolder.GetIndex().Returns(_index);

            _indexService = new IndexService(_stringsSource, _indexHolder);
        }

        [Test]
        public void Get_should_replace_index_holder_exception()
        {
            var occuredException = new IndexIsNotAvailableException();
            _indexHolder.When(x => x.GetIndex())
                        .Do(_ => { throw occuredException; });

            Action action = () => _indexService.Get(1, new int[0]);
            action.ShouldThrow<FaultException>().And.Message.Should().Be(occuredException.Message);
        }

        [Test]
        public void Get_should_replace_line_getter_exception()
        {
            Action action = () => _indexService.Get(5, new int[0]);
            action.ShouldThrow<FaultException>();
        }

        [Test]
        public void Get_should_replace_word_getter_exception()
        {
            Action action = () => _indexService.Get(1, new[] {100});
            action.ShouldThrow<FaultException>();
        }

        [Test]
        public void Get_should_load_line_from_string_source()
        {
            const string expectedString = "expected string";
            _stringsSource.ReadString(15, 25).Returns(expectedString);

            var actualString = _indexService.Get(1, new int[0]);

            actualString.Should().Be(expectedString);
        }

        [Test]
        public void Get_should_limit_loading_line_content_with_one_kilobyte()
        {
            _index.Add(new Line(27, 2000));

            _indexService.Get(2, new int[0]);
            
            _stringsSource.Received(1).ReadString(27, Volumes.Kilobyte - 1 + 27);
        }

        [Test]
        public void Get_should_load_words_of_specified_line()
        {
            _indexService.Get(1, new[] {1, 2});

            _stringsSource.Received(1).ReadString(18, 20);
            _stringsSource.Received(1).ReadString(21, 23);
        }

        [Test]
        public void Get_should_limit_loading_words_content_with_one_kilobyte()
        {
            var line = new Line(27, 4*Volumes.Kilobyte);

            line.AddWord(27, 2 * Volumes.Kilobyte);

            const long word2Start = 2*Volumes.Kilobyte + 3;
            line.AddWord(word2Start, (long) (3.5*Volumes.Kilobyte));

            _index.Add(line);

            _indexService.Get(2, new[] {0, 1});

            _stringsSource.Received(1).ReadString(27, Volumes.Kilobyte - 1 + 27);
            _stringsSource.Received(1).ReadString(word2Start, word2Start + Volumes.Kilobyte - 1);
        }
    }
}
