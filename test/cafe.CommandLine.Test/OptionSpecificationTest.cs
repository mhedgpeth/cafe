using FluentAssertions;
using Xunit;

namespace cafe.CommandLine.Test
{
    public class OptionSpecificationTest
    {
        private static readonly OptionSpecification ChefRunOptionSpecification = new OptionSpecification(
            OptionValueSpecification.ForCommand("chef"), OptionValueSpecification.ForCommand("run"));

        [Fact]
        public void IsSatisfiedBy_ShouldBeSatisfiedByExactArguments()
        {
            ChefRunOptionSpecification.IsSatisfiedBy(OptionGroupTest.ToCommandArguments("chef", "run"))
                .Should()
                .BeTrue("because the arguments match exactly");
        }

        [Fact]
        public void IsSatisfied_ShouldBeFalseWhenParametersDoNotMatch()
        {
            ChefRunOptionSpecification.IsSatisfiedBy(OptionGroupTest.ToCommandArguments("chef", "version"))
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
            ChefDownloadOptionSpecification.IsSatisfiedBy(Argument.CreateCommand("chef"),
                    Argument.CreateCommand("download"), new ValueArgument("version:", "1.2.3"))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseForMissingParameter()
        {
            ChefDownloadOptionSpecification.IsSatisfiedBy(OptionGroupTest.ToCommandArguments("chef", "download"))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseForUnmatchingParameter()
        {
            ChefDownloadOptionSpecification
                .IsSatisfiedBy(OptionGroupTest.ToCommandArguments("chef", "download", "something"))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ParseArguments_ShouldParseDownloadSpecificationCorrectly()
        {
            var version = "1.2.3";
            var arguments = ChefDownloadOptionSpecification.ParseArguments("chef", "download", version);

            arguments.Length.Should().Be(3, "because there are three arguments");
            OptionValueSpecificationTest.AssertArgumentIsCommandArgument("chef", arguments[0]);
            OptionValueSpecificationTest.AssertArgumentIsCommandArgument("download", arguments[1]);
            OptionValueSpecificationTest.AssertArgumentIsValueArgument("version:", version, arguments[2]);
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeTrueForOptionalArgumentThatIsNotThere()
        {
            var specification = new OptionSpecification(OptionValueSpecification.ForCommand("required"),
                OptionValueSpecification.ForOptionalCommand("optional"));

            specification.IsSatisfiedBy(Argument.CreateCommand("required"))
                .Should()
                .BeTrue("because the command is optional");
        }

        [Fact]
        public void ParseArguments_ShouldReturnNullWhenRequiredArgumentsExistButNotInTheSpecification()
        {
            new OptionSpecification("help")
                .ParseArguments("chef", "run")
                .Should()
                .BeNull("because arguments were given that weren't in the specification");
        }

        [Fact]
        public void ParseArguments_ShouldParseCommandAndTwoLabels()
        {
            const string groupLabel = "group:";
            const string policyLabel = "policy:";
            var specification = new OptionSpecification(OptionValueSpecification.ForCommand("chef"),
                OptionValueSpecification.ForValue(policyLabel, "the policy"),
                OptionValueSpecification.ForValue(groupLabel, "the group"));

            const string policyValue = "policy-value";
            const string groupValue = "group-value";
            var arguments = specification.ParseArguments("chef", policyLabel, policyValue, groupLabel, groupValue);

            arguments.Should().NotBeNull("because the arguments fit the specification");
            arguments.Length.Should().Be(3);
            OptionValueSpecificationTest.AssertArgumentIsCommandArgument("chef", arguments[0]);
            OptionValueSpecificationTest.AssertArgumentIsValueArgument(policyLabel, policyValue, arguments[1]);
            OptionValueSpecificationTest.AssertArgumentIsValueArgument(groupLabel, groupValue, arguments[2]);
        }
    }
}