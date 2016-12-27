using System;
using cafe.CommandLine;
using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class OptionTest
    {
        [Fact]
        public void Run_ShouldRecoverFromException()
        {
            var optionThrowingException = new OptionThrowingException();

            var result = optionThrowingException.Run();

            result.IsSuccess.Should().BeFalse("because an exception was thrown while this tried to run");
        }
    }

    public class OptionThrowingException : Option
    {
        public OptionThrowingException() : base(new OptionSpecification("throw"), "throws an exception")
        {
        }

        protected override Result RunCore(string[] args)
        {
            throw new ArgumentException("this option always throws an exception for testing");
        }
    }
}