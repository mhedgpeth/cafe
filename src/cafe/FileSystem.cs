using System.IO;
using Microsoft.Extensions.Logging;

namespace cafe
{
    public interface IFileSystem
    {
        void EnsureDirectoryExists(string directory);
        bool FileExists(string filename);
    }

    public class FileSystem : IFileSystem
    {
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<FileSystem>();

        public void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Logger.LogInformation($"Creating staging directory {directory}");
                Directory.CreateDirectory(directory);
            }
            else
            {
                Logger.LogDebug($"Directory {directory} already exists, so it does not need to be created");
            }
        }

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }
    }
}