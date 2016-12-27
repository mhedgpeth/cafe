using System.IO;
using NLog;

namespace cafe.LocalSystem
{
    public class FileSystemCommandsBoundary : IFileSystemCommands
    {
               private static readonly Logger Logger = LogManager.GetLogger(typeof(FileSystemCommandsBoundary).FullName);

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
            Logger.Debug($"File {filename} exists? {fileExists}");
            return fileExists;
        }

    }
}