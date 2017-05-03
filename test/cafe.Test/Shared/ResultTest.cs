using cafe.CommandLine;
using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Shared
{
    public class ResultTest
    {
        [Fact]
        public void Equals_ShouldBeTrueWhenBothSuccessful()
        {
            Result.Successful().Should().Be(Result.Successful());
        }

        [Fact]
        public void Equals_ShouldBeFalseWhenSuccessfulAgainstFailed()
        {
            Result.Successful().Should().NotBe(Result.Failure("failed!"));
        }

        const string FailureDescription = "Failed!";

        [Fact]
        public void Equals_ShouldBeTrueWhenBothFailed()
        {
            Result.Failure(FailureDescription).Should().Be(Result.Failure(FailureDescription));
        }

        [Fact]
        public void Equals_ShouldBeFalseWhenFailureDescriptionDiffers()
        {
            Result.Failure("one").Should().NotBe(Result.Failure("another"), "because the descriptions differ");
        }

        [Fact]
        public void ToString_ShouldBeDescriptiveWhenSuccessful()
        {
            Result.Successful().ToString().Should().Be("Successful");
        }

        [Fact]
        public void ToString_ShouldBeDescriptiveWhenFailed()
        {

            Result.Failure(FailureDescription).ToString().Should().Be($"Failed: {FailureDescription}");
        }

        [Fact]
        public void TranslateIfFailed_ShouldDoNothingIfPassed()
        {
            var success = Result.Successful().TranslateIfFailed("this should have no effect");

            success.IsSuccess.Should().BeTrue("because status should be preserved when translating");
            success.FailureDescription.Should()
                .BeNullOrEmpty("because the translation shouldn't affect successful results");
        }

        [Fact]
        public void TranslateIfFailed_ShouldTranslateFailureIfFailed()
        {
            const string translation = "translation";
            var failure = Result.Failure("failed!").TranslateIfFailed(translation);

            failure.IsSuccess.Should().BeFalse("because status should be preserved");
            failure.FailureDescription.Should().Be(translation);
        }
    }
}