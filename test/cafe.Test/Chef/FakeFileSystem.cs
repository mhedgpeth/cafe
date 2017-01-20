using cafe.LocalSystem;

namespace cafe.Test.Chef
{
    public class FakeFileSystem : IFileSystem
    {
        public void EnsureDirectoryExists(string directory)
        {
        }

        public string FindInstallationDirectoryInPathContaining(string executable, string defaultPath)
        {
            return executable;
        }

        public bool FileExists(string destination)
        {
            return false;
        }
    }
}