using cafe.CommandLine;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class OptionSpecificationTest
    {
        public static readonly OptionSpecification ChefRunOptionSpecification = new OptionSpecification("chef", "run");

        public static readonly OptionSpecification ChefVersionOptionSpecification = new OptionSpecification("chef",
            "version");

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
            OptionValueSpecification.ForExactValue("chef"),
            OptionValueSpecification.ForExactValue("download"), OptionValueSpecification.ForVersion());


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
    }
}