using System;
using System.IO;

namespace cafe.Chef
{
    public class ChefDownloadUrlResolver : IDownloadUrlResolver
    {
        private readonly string _product;
        private readonly string _prefix;
        private readonly string _target;

        public ChefDownloadUrlResolver(string product, string prefix, string target)
        {
            _product = product;
            _prefix = prefix;
            _target = target;
        }

        public Uri DownloadUriFor(string version)
        {
            // TODO: sanitize data so it can't be injected here
            return new Uri(
                $"https://packages.chef.io/files/stable/{_product}/{version}/windows/{_target}/{FilenameFor(version)}");
        }

        public string FilenameFor(string version)
        {
            // TODO: sanitize data so it can't be injected here
            return $@"{_prefix}-{version}-1-x64.msi";
        }
    }
}