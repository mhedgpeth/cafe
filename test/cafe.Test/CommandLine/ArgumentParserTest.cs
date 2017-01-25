using cafe.CommandLine;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class ArgumentParserTest
    {
        [Fact]
        public void Parse_ShouldParseSingleCommandArgument()
        {
            var expected = "command";
            var arguments = ArgumentParser.Parse(expected);

            arguments.Length.Should().Be(1, "because a single command argument was passed");
            AssertArgumentIsCommandArgument(expected, arguments[0]);
        }

        public static void AssertArgumentIsCommandArgument(string expected, Argument argument)
        {
            var commandArgument = argument as CommandArgument;
            commandArgument.Should().NotBeNull("because a we are expecting a command");
            commandArgument.Command.Should().Be(expected);
        }

        [Fact]
        public void Parse_ShouldParseMultipleCommands()
        {
            var first = "command1";
            var second = "command2";

            var arguments = ArgumentParser.Parse(first, second);

            AssertArgumentIsCommandArgument(first, arguments[0]);
            AssertArgumentIsCommandArgument(second, arguments[1]);
        }

        [Fact]
        public void Parse_ShouldParseValueArgument()
        {
            const string label = "policy:";
            const string value = "webserver";

            var arguments = ArgumentParser.Parse(label, value);

            arguments.Length.Should().Be(1, "because a value should be parsed into one argument");
            var argument = arguments[0] as ValueArgument;
            argument.Label.Should().Be(label);
            argument.Value.Should().Be(value);
        }

        public static void AssertArgumentIsValueArgument(string label, string value, Argument argument)
        {
            var valueArgument = argument as ValueArgument;
            valueArgument.Should().NotBeNull();
            valueArgument.Label.Should().Be(label);
            valueArgument.Value.Should().Be(value);
        }
    }

    public class ArgumentEnumerableExtensionsTest
    {
        [Fact]
        public void FindValueFromLabel_ShouldFindWhenExists()
        {
            var valueArgument = new ValueArgument("policy:", "mywebserver");
            var arguments = new[] {valueArgument};

            arguments.FindValueFromLabel(valueArgument.Label).Should().BeSameAs(valueArgument);
        }

        [Fact]
        public void FindValueFromLabel_ShouldNotWhenExists()
        {
            new Argument[0].FindValueFromLabel("anything:").Should().BeNull("because the label doesn't exist");
        }
    }
}