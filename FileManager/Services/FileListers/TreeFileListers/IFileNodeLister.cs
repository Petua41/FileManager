using FileManager.Models;
using System.Collections.Generic;
using System.IO;

namespace FileManager.Services.FileListers.TreeFileListers
{
    internal interface IFileNodeLister
    {
        List<Node<FileSystemInfo>> GetFileList(DirectoryInfo currentDir);

        IEnumerable<Node<FileSystemInfo>> GetDirectoryNode(DirectoryInfo currentDir);
    }
}
