using System.Diagnostics;
using cafe.LocalSystem;

namespace cafe.Chef
{
    public class ChefUpgrader
    {
        private readonly IFileSystem _fileSystem;
        public ChefUpgrader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Upgrade(string version)
        {
            var msiExecDirectory = _fileSystem.FindInstallationDirectoryInPathContaining("msiexec.exe");

        }
    }
}