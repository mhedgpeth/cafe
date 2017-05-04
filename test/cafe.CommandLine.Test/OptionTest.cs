using System;
using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.CommandLine.Test
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
        public OptionThrowingException() : base("help")
        {
        }

        protected override Result RunCore(Argument[] args)
        {
            throw new ArgumentException("this option always throws an exception for testing");
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Option Throwing Exception";
        }
    }
}