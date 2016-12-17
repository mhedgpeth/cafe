using System.IO;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace cafe.LocalSystem
{
    public interface IFileSystemCommands
    {
        bool DirectoryExists(string directory);
        void CreateDirectory(string directory);
        bool FileExists(string filename);
    }
    public class FileSystemCommandsBoundary : IFileSystemCommands
    {
        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<FileSystemCommandsBoundary>();

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public bool FileExists(string filename)
        {
            var fileExists = File.Exists(filename);
            Logger.LogDebug($"File {filename} exists? {fileExists}");
            return fileExists;
        }

    }

    public class FileSystem : IFileSystem
    {
        private readonly IEnvironment _environment;
        private readonly IFileSystemCommands _commands;

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<FileSystem>();

        public FileSystem(IEnvironment environment, IFileSystemCommands commands)
        {
            _environment = environment;
            _commands = commands;
        }

        public void EnsureDirectoryExists(string directory)
        {
            if (!_commands.DirectoryExists(directory))
            {
                Logger.LogInformation($"Creating staging directory {directory}");
                _commands.CreateDirectory(directory);
            }
            else
            {
                Logger.LogDebug($"Directory {directory} already exists, so it does not need to be created");
            }
        }


        public string FindInstallationDirectoryInPathContaining(string executable)
        {
            var environmentPath = _environment.GetEnvironmentVariable("PATH");
            var paths = environmentPath.Split(';');
            var batchFilePath = paths
                .Select(x => Path.Combine(x, executable))
                .FirstOrDefault(_commands.FileExists);
            if (batchFilePath == null)
            {
                Logger.LogWarning($"Could not find {executable} in the path {environmentPath}");
                return null;
            }
            var binDirectory = Directory.GetParent(batchFilePath);
            return binDirectory.FullName;
        }

    }
}