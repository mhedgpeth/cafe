using Xunit;
using FluentAssertions;
using cafe.CommandLine;

namespace cafe.Test.CommandLine
{
    public class OptionValueSpecificationTest
    {
        [Fact]
        public void IsSatisfiedBy_ShouldBeTrueIfStringIsExactValue()
        {
            const string value = "value";
            var specification = OptionValueSpecification.ForCommand(value);

            specification.IsSatisfiedBy(0, Argument.CreateCommand(value)).Should().BeTrue();
        }

        [Fact]
        public void IsSatisfied_ShouldBeFalseForValueThatDoesNotMatch()
        {
            OptionValueSpecification.ForCommand("something")
                .IsSatisfiedBy(0, Argument.CreateCommand("by a different value"))
                .Should()
                .BeFalse("because the value doesn't exactly match");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseIfExactValueDoesNotMatchButContainsOtherValue()
        {
            OptionValueSpecification.ForCommand("scheduler")
                .IsSatisfiedBy(0, Argument.CreateCommand("schedulers"))
                .Should()
                .BeFalse("because the values aren't exactly the same");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeTrueForAnyValue()
        {
            var value = new ValueArgument("value:", "anything");
            OptionValueSpecification.ForValue("value:", "any value").IsSatisfiedBy(0, value).Should().BeTrue();
        }


        const string Command = "chef";

        [Fact]
        public void ParseArgument_ShouldParseCommandArgument()
        {
            var specification = OptionValueSpecification.ForCommand(Command);

            var argument = specification.ParseArgument(null, Command);
            AssertArgumentIsCommandArgument(Command, argument);
        }

        public static void AssertArgumentIsCommandArgument(string command, Argument argument)
        {
            var commandArgument = argument as CommandArgument;
            commandArgument.Should().NotBeNull();
            commandArgument.Command.Should().Be(command);
        }

        [Fact]
        public void ParseArgument_ShouldParseValueArgument()
        {
            const string label = "version:";
            var specification = OptionValueSpecification.ForValue(label, "description");

            const string value = "1.2.3";
            var argument = specification.ParseArgument(label, value);

            AssertArgumentIsValueArgument(label, value, argument);
        }

        public static void AssertArgumentIsValueArgument(string label, string value, Argument argument)
        {
            var valueArgument = argument as ValueArgument;
            valueArgument.Should().NotBeNull("because expecting argument to be a value argument");
            valueArgument.Label.Should().Be(label);
            valueArgument.Value.Should().Be(value);
        }

        [Fact]
        public void ToString_ShouldShowLabel()
        {
            OptionValueSpecification.ForValue("version:", "the version")
                .ToString()
                .Should()
                .Be("version: (the version)");
        }

        [Fact]
        public void ToString_ShouldHaveBracketsWhenOptional()
        {
            var specification = OptionValueSpecification.ForOptionalValue("on:", "server");
            specification.ToString().Should().Be("[on: (server)]");
        }
    }
}