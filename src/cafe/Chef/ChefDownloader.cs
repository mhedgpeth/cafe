using System;
using System.IO;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public interface IDownloader
    {
        string Product { get; }
        Result Download(string version, IMessagePresenter messagePresenter);
    }

    public class Downloader : IDownloader
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Downloader).FullName);

        public const string StagingDirectory = "staging";

        private readonly IFileDownloader _fileDownloader;
        private readonly IFileSystem _fileSystem;
        private readonly string _product;
        private readonly string _prefix;
        private readonly string _target;

        public Downloader(IFileDownloader fileDownloader, IFileSystem fileSystem, string product, string prefix, string target)
        {
            _fileDownloader = fileDownloader;
            _fileSystem = fileSystem;
            _product = product;
            _prefix = prefix;
            _target = target;
        }

        public string Product => _product;

        public Result Download(string version, IMessagePresenter messagePresenter)
        {
            messagePresenter.ShowMessage("Ensuring staging directory exists");
            _fileSystem.EnsureDirectoryExists(StagingDirectory);
            var destination = FullPathToStagedInstaller(version, _prefix);
            if (_fileSystem.FileExists(destination))
            {
                messagePresenter.ShowMessage($"Download msi already exists, so not downloading again");
                return Result.Successful();
            }
            var downloadLink = DownloadUriFor(version, _product, _prefix, _target);
            messagePresenter.ShowMessage($"Downloading {_product} {version} from {downloadLink}");
            var result  = _fileDownloader.Download(downloadLink, destination);
            messagePresenter.ShowMessage($"Finished downloading {_product} {version} from {downloadLink}");
            Logger.Info(result);
            return result.TranslateIfFailed($"Installer for {_product} {version} could not be found at {downloadLink}");
        }


        public static Uri DownloadUriFor(string version, string product, string prefix, string target)
        {
            // TODO: sanitize data so it can't be injected here
            return new Uri(
                $"https://packages.chef.io/files/stable/{product}/{version}/windows/{target}/{FilenameFor(version, prefix)}");
        }

        public static string FilenameFor(string version, string prefix)
        {
            // TODO: sanitize data so it can't be injected here
            return $@"{prefix}-{version}-1-x64.msi";
        }

        public static string FullPathToStagedInstaller(string version, string prefix)
        {
            return Path.Combine(StagingDirectory, FilenameFor(version, prefix));
        }
    }
}