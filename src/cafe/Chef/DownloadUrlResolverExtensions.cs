using System.IO;

namespace cafe.Chef
{
    public static class DownloadUrlResolverExtensions
    {
        public static string FullPathToStagedInstaller(this IDownloadUrlResolver resolver, string version)
        {
            return Path.Combine(Downloader.StagingDirectory, resolver.FilenameFor(version));
        }
    }
}