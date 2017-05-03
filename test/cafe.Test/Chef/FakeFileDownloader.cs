using System;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Test.Chef
{
    public class FakeFileDownloader : IFileDownloader
    {
        public Result Download(Uri downloadLink, string file)
        {
            return Result.Successful();
        }
    }
}