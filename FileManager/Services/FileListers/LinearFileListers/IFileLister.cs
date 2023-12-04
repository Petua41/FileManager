using System.Collections.Generic;
using System.IO;
using FileManager.Infrastructure.Extensions;

namespace FileManager.Services.FileListers.LinearFileListers
{
    internal interface IFileLister
    {
        Dictionary<string, FileType> GetFileList(DirectoryInfo currentDir);
    }
}
