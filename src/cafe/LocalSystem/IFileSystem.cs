namespace cafe.LocalSystem
{
    public interface IFileSystem
    {
        void EnsureDirectoryExists(string directory);
        bool FileExists(string filename);
    }
}