using cafe.CommandLine;
using Xunit;
using FluentAssertions;

namespace cafe.Test.CommandLine
{
    public class RunnerTest
    {
        private FakeOption CreateChefRunOption()
        {
            return new FakeOption(OptionSpecificationTest.ChefRunOptionSpecification, "runs chef");
        }

        [Fact]
        public void Parse_ShouldSelectMatchingOption()
        {
            var option = CreateChefRunOption();
            var runner = new Runner(option);

            runner.Run("chef", "run");

            option.WasRun.Should().BeTrue();
        }

        private FakeOption CreateChefVersionOption()
        {
            return new FakeOption(OptionSpecificationTest.ChefVersionOptionSpecification, "show chef version");
        }

        [Fact]
        public void Parse_ShouldSelectSecondOptionIfFirstDoesNotMatch()
        {
            var chefRunOption = CreateChefRunOption();
            var chefVersionOption = CreateChefVersionOption();
            var runner = new Runner(chefRunOption, chefVersionOption);

            runner.Run("chef", "version");

            chefRunOption.WasRun.Should().BeFalse();
            chefVersionOption.WasRun.Should().BeTrue();
        }

        [Fact]
        public void Parse_ShouldMatchIfHelpIsAskedFor()
        {
            var chefRunOption = CreateChefRunOption();
            var runner = new Runner(chefRunOption);

            runner.Run("chef", "run", "-h");

            chefRunOption.WasRun.Should().BeFalse();
            chefRunOption.WasHelpShown.Should().BeTrue();
        }
    }
}