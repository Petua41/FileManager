using FileManager.Models;
using System;
using System.Collections.Generic;

namespace FileManager.Infrastructure.Comparers
{
    internal class FileRecordComparer : IComparer<FileRecord>   // If another sorting is needed, this class can be substituted
    {
        public int Compare(FileRecord x, FileRecord y)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(x.Filename, y.Filename);
        }
    }
}
