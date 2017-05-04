namespace cafe.CommandLine.LocalSystem
{
    public interface IFileSystemCommands
    {
        bool DirectoryExists(string directory);
        void CreateDirectory(string directory);
        bool FileExists(string filename);
        void WriteFileText(string filename, string contents);
        string ReadAllText(string filename);
        void DeleteDirectory(string directory);
    }
}