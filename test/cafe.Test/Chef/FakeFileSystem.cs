using System.Collections.Generic;
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

    }
}