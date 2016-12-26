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
            var specification = OptionValueSpecification.ForExactValue(value);

            specification.IsSatisfiedBy(value).Should().BeTrue();
        }

        [Fact]
        public void IsSatisfied_ShouldBeFalseForValueThatDoesNotMatch()
        {
            OptionValueSpecification.ForExactValue("something")
                .IsSatisfiedBy("by a different value")
                .Should()
                .BeFalse("because the value doesn't exactly match");
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseIfExactValueDoesNotMatchButContainsOtherValue()
        {
            OptionValueSpecification.ForExactValue("scheduler")
                .IsSatisfiedBy("schedulers")
                .Should()
                .BeFalse("because the values aren't exactly the same");
        }
    }
}