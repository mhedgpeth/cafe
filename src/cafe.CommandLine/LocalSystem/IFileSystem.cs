namespace cafe.CommandLine.LocalSystem
{
    public interface IFileSystem
    {
        void EnsureDirectoryExists(string directory);
        string FindInstallationDirectoryInPathContaining(string executable, string defaultPath);
        bool FileExists(string destination);
    }
}