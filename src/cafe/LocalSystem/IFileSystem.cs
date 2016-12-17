namespace cafe.LocalSystem
{
    public interface IFileSystem
    {
        void EnsureDirectoryExists(string directory);
        string FindInstallationDirectoryInPathContaining(string executable);
    }
}