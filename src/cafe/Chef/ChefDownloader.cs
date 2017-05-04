using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class Downloader : IDownloader
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Downloader).FullName);

        public const string StagingDirectory = "staging";

        private readonly IFileDownloader _fileDownloader;
        private readonly IFileSystem _fileSystem;
        private readonly string _product;
        private readonly IDownloadUrlResolver _downloadUrlResolver;

        public Downloader(IFileDownloader fileDownloader, IFileSystem fileSystem, string product, IDownloadUrlResolver downloadUrlResolver)
        {
            _fileDownloader = fileDownloader;
            _fileSystem = fileSystem;
            _product = product;
            _downloadUrlResolver = downloadUrlResolver;
        }

        public string Product => _product;

        public Result Download(string version, IMessagePresenter messagePresenter)
        {
            messagePresenter.ShowMessage("Ensuring staging directory exists");
            _fileSystem.EnsureDirectoryExists(StagingDirectory);
            var destination = _downloadUrlResolver.FullPathToStagedInstaller(version);
            if (_fileSystem.FileExists(destination))
            {
                messagePresenter.ShowMessage($"Download msi already exists, so not downloading again");
                return Result.Successful();
            }
            var downloadLink = _downloadUrlResolver.DownloadUriFor(version);
            messagePresenter.ShowMessage($"Downloading {_product} {version} from {downloadLink}");
            var result  = _fileDownloader.Download(downloadLink, destination);
            messagePresenter.ShowMessage($"Finished downloading {_product} {version} from {downloadLink}");
            Logger.Info(result);
            return result.TranslateIfFailed($"Installer for {_product} {version} could not be found at {downloadLink}");
        }
    }
}