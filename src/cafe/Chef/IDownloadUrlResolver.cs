using System;

namespace cafe.Chef
{
    public interface IDownloadUrlResolver
    {
        Uri DownloadUriFor(string version);
        string FilenameFor(string version);
    }
}