using FileManager.Models;
using System.Collections.Generic;
using System.IO;

namespace FileManager.Services.FileListers.TreeFileListers
{
    internal class AllFileNodesLister : TreeFileLister
    {
        public override List<Node<FileSystemInfo>> GetFileList(DirectoryInfo currentDirectory)
        {
            return GetAllFileNodes(currentDirectory);
        }
    }
}
