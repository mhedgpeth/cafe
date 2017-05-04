using System.IO;
using NLog;

namespace cafe.CommandLine.LocalSystem
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

        public void WriteFileText(string filename, string contents)
        {
            Logger.Debug($"Writing contents to {filename}");
            File.WriteAllText(filename, contents);
        }

        public string ReadAllText(string filename)
        {
            Logger.Debug($"Reading contents from {filename}");
            return File.ReadAllText(filename);
        }

        public void DeleteDirectory(string directory)
        {
            Logger.Debug($"Removing directory {directory}");
            Directory.Delete(directory, true);
        }
    }
}