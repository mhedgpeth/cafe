using System;
using System.IO;
using cafe.LocalSystem;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public interface IDownloader
    {
        Result Download(string version, IMessagePresenter messagePresenter);
    }

    public class Downloader : IDownloader
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Downloader).FullName);

        public const string StagingDirectory = "staging";

        private readonly IFileDownloader _fileDownloader;
        private readonly IFileSystem _fileSystem;

        public Downloader(IFileDownloader fileDownloader, IFileSystem fileSystem)
        {
            _fileDownloader = fileDownloader;
            _fileSystem = fileSystem;
        }

        public Result Download(string version, IMessagePresenter messagePresenter)
        {
            messagePresenter.ShowMessage("Ensuring staging directory exists");
            _fileSystem.EnsureDirectoryExists(StagingDirectory);
            var downloadLink = DownloadUriFor(version);
            messagePresenter.ShowMessage($"Downloading Chef {version} from {downloadLink}");
            var file = FilenameFor(version);
            var result  = _fileDownloader.Download(downloadLink, FullPathToStagedInstaller(version));
            messagePresenter.ShowMessage($"Finished downloading Chef {version} from {downloadLink}");
            Logger.Info(result);
            return result.TranslateIfFailed($"Installer for Chef {version} could not be found at {downloadLink}");
        }


        public static Uri DownloadUriFor(string version)
        {
            // TODO: sanitize data so it can't be injected here
            return new Uri(
                $"https://packages.chef.io/files/stable/chef/{version}/windows/2012r2/{FilenameFor(version)}");
        }

        public static string FilenameFor(string version)
        {
            // TODO: sanitize data so it can't be injected here
            return $@"chef-client-{version}-1-x64.msi";
        }

        public static string FullPathToStagedInstaller(string version)
        {
            return Path.Combine(StagingDirectory, FilenameFor(version));
        }
    }
}