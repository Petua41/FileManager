using FileManager.Infrastructure.Collections;
using FileManager.Services.FileListers.LinearFileListers;
using FileManager.Services.FileListers.TreeFileListers;
using FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using FileManager.Infrastructure.Extensions;

namespace FileManager.Services
{
    public static class CurrentDirectory
    {
        private const int RECENT_CAPACITY = 3;

        private static IFileLister fileListingStrategy;
        private static IFileNodeLister fileNodeListingStrategy;
        private static readonly LimitedStack<DirectoryInfo> recent = new(RECENT_CAPACITY);
        private static DirectoryInfo currentDir;

        static CurrentDirectory()
        {
            string? currentDirString = Path.GetPathRoot(Environment.CurrentDirectory);
            if (string.IsNullOrWhiteSpace(currentDirString))
            {
                DialogBoxes.ShowWarningBox("Cannot get root directory! Strating in current working directory...");  // impossible case
                currentDirString = Environment.CurrentDirectory;
            }

            CurrentDir = currentDir = new(currentDirString);
            fileListingStrategy = new OnlyVisibleFilesLister();
            fileNodeListingStrategy = new OnlyVisibleFilesNodesLister();
        }

        public static void SetFileListingStrategy(bool showHidden)
        {
            fileListingStrategy = showHidden
                ? new AllFilesLister()
                : new OnlyVisibleFilesLister();
            fileNodeListingStrategy = showHidden
                ? new AllFileNodesLister()
                : new OnlyVisibleFilesNodesLister();
        }

        public static bool IsRootDirectory()
        {
            return CurrentDir.Parent is null;
        }

        public static void GoUp()
        {
            if (CurrentDir.Parent is not null) CurrentDir = CurrentDir.Parent;
            CurrentDirectorySwitched?.Invoke(null, new());
        }

        public static Dictionary<string, FileType> GetFiles() => fileListingStrategy.GetFileList(CurrentDir);

        public static List<Node<FileSystemInfo>> GetFileNodes() => fileNodeListingStrategy.GetFileList(CurrentDir);

        public static IEnumerable<Node<FileSystemInfo>> GetNode()
        {
            foreach (Node<FileSystemInfo> nodeVersion in fileNodeListingStrategy.GetFileList(CurrentDir)) yield return nodeVersion;
        }

        public static void Go(string path)
        {
            if (Directory.Exists(path))
            {
                CurrentDir = new(path);
                CurrentDirectorySwitched?.Invoke(null, new());
            }
            else
            {
                throw new DirectoryNotFoundException($"Cannot set current directory to {path}: directory not found");
            }
        }

        public static string Name => CurrentDir.FullName;

        public static DirectoryInfo CurrentDir
        {
            get => currentDir;
            private set
            {
                currentDir = value;
                if (!recent.Contains(currentDir)) recent.Push(currentDir);
            }
        }

        public static IReadOnlyList<DirectoryInfo> Recent => recent.ToList();

        public static event EventHandler? CurrentDirectorySwitched;
    }
}
