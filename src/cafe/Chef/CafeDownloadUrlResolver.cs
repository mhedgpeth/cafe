using System;

namespace cafe.Chef
{
    public class CafeDownloadUrlResolver : IDownloadUrlResolver
    {
        private readonly string _platform;

        public CafeDownloadUrlResolver(string platform)
        {
            _platform = platform;
        }

        public Uri DownloadUriFor(string version)
        {
            string githubVersion = version.StartsWith("0.") ? version + "-beta" : version;
            return new Uri($"https://github.com/mhedgpeth/cafe/releases/download/{githubVersion}/{FilenameFor(version)}");
        }

        public string FilenameFor(string version)
        {
            return $"cafe-{_platform}-x64-{version}.0.zip";
        }

    }
}