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

            specification.IsSatisfiedBy(0, value).Should().BeTrue();
        }

        [Fact]
        public void IsSatisfied_ShouldBeFalseForValueThatDoesNotMatch()
        {
            OptionValueSpecification.ForCommand("something")
                .IsSatisfiedBy(0, "by a different value")
                .Should()
                .BeFalse("because the value doesn't exactly match");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseIfExactValueDoesNotMatchButContainsOtherValue()
        {
            OptionValueSpecification.ForCommand("scheduler")
                .IsSatisfiedBy(0, "schedulers")
                .Should()
                .BeFalse("because the values aren't exactly the same");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeTrueForAnyValue()
        {
            OptionValueSpecification.ForValue("value:", "any value").IsSatisfiedBy(0, "policy").Should().BeTrue();
        }


        const string Command = "chef";

        [Fact]
        public void ParseArgument_ShouldParseCommandArgument()
        {
            var specification = OptionValueSpecification.ForCommand(Command);

            var argument = specification.ParseArgument(0, Command);
            ArgumentParserTest.AssertArgumentIsCommandArgument(Command, argument);
        }

        [Fact]
        public void ParseArgument_ShouldParseCommandArgumentAtLaterPosition()
        {
            var specification = OptionValueSpecification.ForCommand(Command);

            var argument = specification.ParseArgument(1, "something", Command);

            ArgumentParserTest.AssertArgumentIsCommandArgument(Command, argument);
        }

        [Fact]
        public void ParseArgument_ShouldParseValueArgument()
        {
            const string label = "version:";
            var specification = OptionValueSpecification.ForValue(label, "description");

            const string value = "1.2.3";
            var argument = specification.ParseArgument(0, value);

            ArgumentParserTest.AssertArgumentIsValueArgument(label, value, argument);
        }
    }
}