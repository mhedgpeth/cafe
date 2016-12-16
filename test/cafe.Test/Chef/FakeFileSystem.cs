using System.Collections.Generic;
using cafe.LocalSystem;

namespace cafe.Test.Chef
{
    public class FakeFileSystem : IFileSystem
    {
        public void EnsureDirectoryExists(string directory)
        {
        }

        public bool FileExists(string filename)
        {
            return ExistingFiles.Contains(filename);
        }

        public static FakeFileSystem CreateWithExistingFiles(params string[] files)
        {
            return new FakeFileSystem() {ExistingFiles = new List<string>(files)};
        }

        public List<string> ExistingFiles { get; set; } = new List<string>();
    }
}