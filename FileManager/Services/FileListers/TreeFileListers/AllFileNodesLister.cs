using FileManager.Infrastructure.Extensions;
using FileManager.Models;
using System.Collections.Generic;
using System.IO;

namespace FileManager.Services.FileListers.TreeFileListers
{
    internal class AllFileNodesLister : TreeFileLister
    {
        public async override IAsyncEnumerable<Node<FileSystemInfo>> GetDirectoryNode(DirectoryInfo currentDirectory)
        {
            await foreach (Node<FileSystemInfo> rootVersion in GetRootNodeCoroutine(currentDirectory)) yield return rootVersion;
        }

        public override List<Node<FileSystemInfo>> GetFileList(DirectoryInfo currentDirectory)
        {
            return GetAllFileNodes(currentDirectory);
        }
    }
}
