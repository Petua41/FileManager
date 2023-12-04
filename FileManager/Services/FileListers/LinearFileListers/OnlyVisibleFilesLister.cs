using System.Collections.Generic;
using System.IO;
using FileManager.Infrastructure.Extensions;

namespace FileManager.Services.FileListers.LinearFileListers
{
    internal class OnlyVisibleFilesLister : FileLister
    {
        public override Dictionary<string, FileType> GetFileList(DirectoryInfo currentDir)
        {
            return GetAllFilesInDirectory(currentDir, fs => !fs.IsHidden());
        }
    }
}
