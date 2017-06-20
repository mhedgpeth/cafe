using System;

namespace cafe.Shared
{
    public static class VersionMatcher
    {
        
        public static bool DoVersionsMatch(string expectedVersion, string actualVersion)
        {
            var expected = Version.Parse(expectedVersion);
            var actual = Version.Parse(actualVersion);
            return expected.Major == actual.Major && expected.Minor == actual.Minor && expected.Build == actual.Build;
        }
    }
}