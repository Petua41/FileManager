using System.IO;
using System.Linq;
using FileManager.Infrastructure.Extensions;

namespace FileManager.Services
{
    internal class StatisticsCalculator(DirectoryInfo directory)
    {
        public int CountFiles()
        {
            return Files.Length;
        }

        public int CountDirectories()
        {
            return Directories.Length;
        }

        public int CountTotal()
        {
            return FSInfos.Length;
        }

        public int CountHiddenFiles()
        {
            return Files.Where(FileSystemExtensions.IsHidden).Count();
        }

        public int CountHiddenDirectories()
        {
            return Directories.Where(FileSystemExtensions.IsHidden).Count();
        }

        public int CountHiddenTotal()
        {
            return FSInfos.Where(FileSystemExtensions.IsHidden).Count();
        }

        private FileInfo[] Files => directory.GetFiles();       // if we would wanted to calculate size, we`ll need those properties

        private DirectoryInfo[] Directories => directory.GetDirectories();

        private FileSystemInfo[] FSInfos => directory.GetFileSystemInfos();
    }
}
