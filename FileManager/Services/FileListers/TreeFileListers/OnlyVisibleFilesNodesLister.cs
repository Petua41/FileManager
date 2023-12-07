using FileManager.Infrastructure.Extensions;
using FileManager.Models;
using System.Collections.Generic;
using System.IO;

namespace FileManager.Services.FileListers.TreeFileListers
{
    internal class OnlyVisibleFilesNodesLister : TreeFileLister
    {
        public async override IAsyncEnumerable<Node<FileSystemInfo>> GetDirectoryNode(DirectoryInfo currentDirectory)
        {
            await foreach (Node<FileSystemInfo> rootVersion in GetRootNodeCoroutine(currentDirectory, fs => !fs.IsHidden())) yield return rootVersion;
        }

        public override List<Node<FileSystemInfo>> GetFileList(DirectoryInfo currentDirectory)
        {
            return GetAllFileNodes(currentDirectory, fs => !fs.IsHidden());
        }
    }
}
