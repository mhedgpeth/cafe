using System.Text.RegularExpressions;
using cafe.CommandLine;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class LabeledValueSpecificationTest
    {
        private const string Label = "version:";
        private const string Value = "1.2.3";

        [Fact]
        public void ParseArgument_ShouldParseArgumentWithoutLabel()
        {
            var specification = CreateLabeledValueSpecification();

            var argument = specification.ParseArgument(Label, Value);

            OptionValueSpecificationTest.AssertArgumentIsValueArgument(Label, Value, argument);
        }

        private LabelledValueSpecification CreateLabeledValueSpecification()
        {
            return new AnyLabelledValueSpecification(Label, "does something awesome");
        }

        [Fact]
        public void ParseArgument_ShouldParseArgumentWithLabel()
        {
            var specification = CreateLabeledValueSpecification();

            var argument = specification.ParseArgument(Label, Value);

            OptionValueSpecificationTest.AssertArgumentIsValueArgument(Label, Value, argument);
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseIfItDoesNotMatchRegularExpression()
        {
            var versionRegularExpression = new Regex(@"\d+\.\d+\.\d+");
            var specification = new MatchingRegularExpressionLabelledValueSpecification(Label, versionRegularExpression, "labeled value");

            specification.IsSatisfiedBy(0, Argument.CreateCommand("invalid-value"))
                .Should()
                .BeFalse("because the value doesn't match the specification value");
        }

        [Fact]
        public void ParseArgument_ShouldReturnNullIfLabelDoesNotMatch()
        {
            var specification = CreateLabeledValueSpecification();
            specification.ParseArgument("another:", "value").Should().BeNull("because the label doesn't match");
        }
    }
}