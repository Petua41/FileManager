using FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileManager.Services.FileListers.TreeFileListers
{
    internal abstract class TreeFileLister() : IFileNodeLister
    {
        private Predicate<FileSystemInfo>? savedPredicate = null;

        public abstract List<Node<FileSystemInfo>> GetFileList(DirectoryInfo currentDirectory);

        protected List<Node<FileSystemInfo>> GetAllFileNodes(DirectoryInfo directory, Predicate<FileSystemInfo>? predicate = null)
        {
            if (savedPredicate is null && predicate is not null) savedPredicate = predicate;

            List<Node<FileSystemInfo>> result = [];
            foreach (FileSystemInfo fsInfo in directory.EnumerateFileSystemInfos())
            {
                if (savedPredicate is null || savedPredicate(fsInfo)) 
                {
                    result.Add(CreateNode(fsInfo));
                }
            }

            return result;
        }

        private Node<FileSystemInfo> CreateNode(FileSystemInfo fsInfo)
        {
            if (fsInfo is DirectoryInfo dirInfo)
            {
                try
                {
                    return new(dirInfo, GetAllFileNodes(dirInfo));
                }
                catch (Exception e) when (e is UnauthorizedAccessException or IOException) { /* return */ }
            }
            return new(fsInfo);
        }
    }
}
