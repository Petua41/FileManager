using System;
using System.Collections.Generic;
using System.IO;
using FileManager.Infrastructure.Extensions;

namespace FileManager.Services.FileListers.LinearFileListers
{
    internal abstract class FileLister : IFileLister
    {
        public abstract Dictionary<string, FileType> GetFileList(DirectoryInfo currentDir);

        protected Dictionary<string, FileType> GetAllFilesInDirectory(DirectoryInfo directory, Predicate<FileSystemInfo>? predicate = null)
        // FileSystemInfo contains information for all possible predicates. FileType cannot check if file, for example, hidden
        {
            Dictionary<string, FileType> result = [];

            foreach (FileSystemInfo fileSystem in directory.GetFileSystemInfos())      // GetFileSystemInfos lists all (files and directories)
            {
                if (predicate is not null && !predicate(fileSystem)) continue;

                string filename = fileSystem.Name;
                if (!result.TryAdd(filename, fileSystem.GetFileType()))
                {
                    DialogBoxes.ShowWarningBox($"Several files with the same name: {filename}. Showed only first copy");    // impossible case: files in any file system cannot have same name
                }
            }

            return result;
        }
    }
}
