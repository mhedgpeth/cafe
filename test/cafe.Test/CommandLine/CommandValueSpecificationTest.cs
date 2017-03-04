using cafe.CommandLine;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class ommandValueSpecificationTest
    {
        private const string Command = "chef";

        [Fact]
        public void ParseArgument_ShouldParseCommandIfItExistsInArgument()
        {
            var specification = CreateSpecification();

            var argument = specification.ParseArgument(null, Command);

            OptionValueSpecificationTest.AssertArgumentIsCommandArgument(Command, argument);
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
                .ParseArgument(null, "another")
                .Should()
                .BeNull("because the command doesn't exist in the argument list");
        }
    }
}