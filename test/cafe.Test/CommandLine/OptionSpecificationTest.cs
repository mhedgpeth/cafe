using System.Text.RegularExpressions;
using cafe.CommandLine;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class OptionSpecificationTest
    {
        private static readonly OptionSpecification ChefRunOptionSpecification = new OptionSpecification("chef", "run");

        [Fact]
        public void IsSatisfiedBy_ShouldBeSatisfiedByExactArguments()
        {
            ChefRunOptionSpecification.IsSatisfiedBy("chef", "run")
                .Should()
                .BeTrue("because the arguments match exactly");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeSatisfiedByHelpArguments()
        {
            ChefRunOptionSpecification.IsSatisfiedBy("chef", "run", "-h")
                .Should()
                .BeTrue("because the options match and they are asking for help");
        }

        [Fact]
        public void IsSatisfied_ShouldBeFalseWhenParametersDoNotMatch()
        {
            ChefRunOptionSpecification.IsSatisfiedBy("chef", "version")
                .Should()
                .BeFalse("because parameters don't match");
        }

        private static readonly OptionSpecification ChefDownloadOptionSpecification = new OptionSpecification(
            OptionValueSpecification.ForCommand("chef"),
            OptionValueSpecification.ForCommand("download"),
            OptionValueSpecification.ForVersion());


        [Fact]
        public void IsSatisfied_ShouldBeTrueWhenParameterized()
        {
            ChefDownloadOptionSpecification.IsSatisfiedBy("chef", "download", "1.2.3").Should().BeTrue();
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseForMissingParameter()
        {
            ChefDownloadOptionSpecification.IsSatisfiedBy("chef", "download").Should().BeFalse();
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseForUnmatchingParameter()
        {
            ChefDownloadOptionSpecification.IsSatisfiedBy("chef", "download", "something").Should().BeFalse();
        }

        [Fact]
        public void ParseArguments_ShouldParseDownloadSpecificationCorrectly()
        {
            var version = "1.2.3";
            var arguments = ChefDownloadOptionSpecification.ParseArguments("chef", "download", version);

            arguments.Length.Should().Be(3, "because there are three arguments");
            ArgumentParserTest.AssertArgumentIsCommandArgument("chef", arguments[0]);
            ArgumentParserTest.AssertArgumentIsCommandArgument("download", arguments[1]);
            ArgumentParserTest.AssertArgumentIsValueArgument("version:", version, arguments[2]);
        }
    }

    public class CommandValueSpecificationTest
    {
        private const string Command = "chef";

        [Fact]
        public void ParseArgument_ShouldParseCommandIfItExistsInArgument()
        {
            var specification = CreateSpecification();

            var argument = specification.ParseArgument(0, Command);

            ArgumentParserTest.AssertArgumentIsCommandArgument(Command, argument);
        }

        private static CommandOptionValueSpecification CreateSpecification()
        {
            var specification = new CommandOptionValueSpecification(Command, "a command");
            return specification;
        }

        [Fact]
        public void ParseArgument_ShouldReturnNullIfCommandDoesNotExistInArguments()
        {
            CreateSpecification()
                .ParseArgument(0, "another")
                .Should()
                .BeNull("because the command doesn't exist in the argument list");
        }

        [Fact]
        public void ParseArgument_ShouldReturnNullIfCommandAtPositionDoesNotExistButExistsElsewhere()
        {
            CreateSpecification()
                .ParseArgument(0, "another", Command)
                .Should()
                .BeNull("because the command doesn't exist in the right location");
        }
    }

    public class LabeledValueSpecificationTest
    {
        private const string Label = "version:";
        private const string Value = "1.2.3";

        [Fact]
        public void ParseArgument_ShouldParseArgumentWithoutLabel()
        {
            var specification = CreateLabeledValueSpecification();

            var argument = specification.ParseArgument(0, Value);

            ArgumentParserTest.AssertArgumentIsValueArgument(Label, Value, argument);
        }

        private LabelledValueSpecification CreateLabeledValueSpecification()
        {
            return new AnyLabelledValueSpecification(Label, "does something awesome");
        }

        [Fact]
        public void ParseArgument_ShouldParseArgumentWithLabel()
        {
            var specification = CreateLabeledValueSpecification();

            var argument = specification.ParseArgument(0, Label, Value);

            ArgumentParserTest.AssertArgumentIsValueArgument(Label, Value, argument);
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeSatisfiedByArgumentWithoutLabel()
        {
            var specification = CreateLabeledValueSpecification();


            specification.IsSatisfiedBy(0, Value).Should().BeTrue();
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeSatisfiedByArgumentWithLabel()
        {
            CreateLabeledValueSpecification().IsSatisfiedBy(0, Label, Value).Should().BeTrue("because the value is there and labeled properly");

        }

        [Fact]
        public void IsSatsifiedBy_ShouldNotBeSatisfiedByArgumentsWithJustLabel()
        {
            CreateLabeledValueSpecification()
                .IsSatisfiedBy(0, Label)
                .Should()
                .BeFalse("because the label is missing a value");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseIfItDoesNotMatchRegularExpression()
        {
            var versionRegularExpression = new Regex(@"\d+\.\d+\.\d+");
            var specification = new MatchingRegularExpressionLabelledValueSpecification(Label, versionRegularExpression, "labeled value");

            specification.IsSatisfiedBy(0, "invalid-value")
                .Should()
                .BeFalse("because the value doesn't match the specification value");
        }
    }
}