﻿using System;
using FluentAssertions;
using NUnit.Framework;

namespace FileIndexer.Tests
{
    [TestFixture]
    public class CommandParserTests
    {
        private CommandParser _commandParser;

        [SetUp]
        public void SetUp()
        {
            _commandParser = new CommandParser();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t")]
        public void Should_return_null_command_when_command_text_is_empty(string commandText)
        {
            var command = _commandParser.ParseCommandText(commandText);

            command.Should().BeNull();
        }

        [TestCase("exit")]
        [TestCase("exit \t")]
        public void Should_return_exit_command_when_command_starts_with_exit_command(string commandText)
        {
            var command = _commandParser.ParseCommandText(commandText);

            command.Should().BeOfType<ExitCommand>();
        }

        [Test]
        public void Should_fail_when_command_text_starts_with_unknown_command()
        {
            Action action = () => _commandParser.ParseCommandText("unknown command text");

            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Should_parse_get_command_text_with_line_parameter()
        {
            var command = _commandParser.ParseCommandText("get    1 ");

            command.Should().BeOfType<PrintLineWords>();
            command.As<PrintLineWords>().LineIndex.Should().Be(1);
        }

        [Test]
        public void Should_parse_get_command_with_line_parameter_and_words_numbers()
        {
            var command = _commandParser.ParseCommandText("get  2  3   10  12");

            command.Should().BeOfType<PrintLineWords>();
            command.As<PrintLineWords>().LineIndex.Should().Be(2);
            command.As<PrintLineWords>().WordIndexes.Should().BeEquivalentTo(new[] {3, 10, 12});
        }
    }
}
