using FileManager.Models;
using System;
using System.Collections.Generic;

namespace FileManager.Infrastructure.Comparers
{
    internal class FileNodeComparer : IComparer<FileNode>   // If another sorting is needed, this class can be substituted
    {
        public int Compare(FileNode x, FileNode y)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(x.File.Filename, y.File.Filename);
        }
    }
}
