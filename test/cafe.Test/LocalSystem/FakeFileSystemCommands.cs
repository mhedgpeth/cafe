using System.Collections.Generic;
using cafe.LocalSystem;

namespace cafe.Test.LocalSystem
{
    public class FakeFileSystemCommands : IFileSystemCommands
    {
        public bool DirectoryExists(string directory)
        {
            return false;
        }

        public void CreateDirectory(string directory)
        {
        }

        public bool FileExists(string filename)
        {
            return ExistingFiles.Contains(filename);
        }


        public static FakeFileSystemCommands CreateWithExistingFiles(params string[] files)
        {
            return new FakeFileSystemCommands() {ExistingFiles = new List<string>(files)};
        }

        public List<string> ExistingFiles { get; set; } = new List<string>();
        public void WriteFileText(string filename, string contents)
        {
        }

        public string ReadAllText(string filename)
        {
            return string.Empty;
        }

        public void DeleteDirectory(string directory)
        {
        }
    }
}