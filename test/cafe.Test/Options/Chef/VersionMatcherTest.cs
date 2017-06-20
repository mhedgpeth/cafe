using cafe.Options.Chef;
using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Options.Chef
{
    public class VersionMatcherTest
    {
        [Fact]
        public void DoVersionsMatch_ShouldBeTrueWhenMatching()
        {
            var version = "12.2.1";
            VersionMatcher.DoVersionsMatch(version, version).Should()
                .BeTrue("because the versions exactly match");
        }

        [Fact]
        public void DoVersionsMatch_ShouldBeFalseWhenDifferent()
        {
            VersionMatcher.DoVersionsMatch("12.2.1", "12.2.2").Should()
                .BeFalse("because the versions are different");
        }

        [Fact]
        public void DoVersionsMatch_ShouldBeTrueWhenLastDigitDifferent()
        {
            VersionMatcher.DoVersionsMatch("12.2.1", "12.2.1.1").Should()
                .BeTrue("because only the last digit is different");
        }

        [Fact]
        public void DoVersionsMatch_ShouldBeFalseWhenFirstDigitDifferentAndLastDigitDifferent()
        {
            VersionMatcher.DoVersionsMatch("13.2.1", "12.2.1.1").Should()
                .BeFalse("because first digit is different");
        }
    }
}