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

            Node<FileSystemInfo> root = CreateNodeRecursive(directory);

            return [.. root.SubNodes];      // this expression is equivalent of new List<Node<FileSystemInfo>>(root.SubNodes);
        }

        private Node<FileSystemInfo> CreateNodeRecursive(FileSystemInfo fsInfo)
        {
            Node<FileSystemInfo> result = new(fsInfo);

            if (fsInfo is DirectoryInfo dInfo)
            {
                foreach (FileSystemInfo fs in dInfo.EnumerateFileSystemInfos())
                {
                    if (savedPredicate is not null && !savedPredicate(fs)) continue;

                    try
                    {
                        result.SubNodes.Add(CreateNodeRecursive(fs));
                    }
                    catch (Exception e) when (e is UnauthorizedAccessException or IOException) { result.SubNodes.Add(new(fs)); }
                }
            }

            return result;
        }
    }
}
