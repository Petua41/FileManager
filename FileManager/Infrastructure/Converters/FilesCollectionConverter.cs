using FileManager.Infrastructure.Comparers;
using FileManager.Infrastructure.Extensions;
using FileManager.Models;
using FileManager.Services;
using FileManager.Services.FileListers.TreeFileListers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileManager.Infrastructure.Converters
{
    internal static class FilesCollectionConverter
    {
        private static readonly Dictionary<FileType, string> icons = new()
        {
            { FileType.Unknown, "fa-file" },
            { FileType.Directory, "fa-folder" },
            { FileType.Text, "fa-file-lines" },
            { FileType.Image, "fa-image" }
        };

        private const string FADED = "fa-regular ";

        private static readonly Dictionary<string, List<FileRecord>> addedFiles = [];

        static FilesCollectionConverter()
        {
            DriveSyncHandler.StateChanged += OnStateChanged;
        }

        private static void OnStateChanged(object? sender, Events.SyncStateChangeEventArgs e)
        {
            if (e.NewState == SyncState.Synced) addedFiles.Clear();     // they`ve been written to disk
        }

        public static void AddFile(string path)
        {
            string filename = Path.GetFileName(path);
            FileType type = new FileInfo(filename).GetFileType();
            string icon = FADED + icons[type];

            if (addedFiles.TryGetValue(CurrentDirectory.Name, out List<FileRecord>? value)) value.Add(new(filename, icon, type, path));
            else addedFiles.Add(CurrentDirectory.Name, [new(filename, icon, type, path)]);

            TempFileAdded?.Invoke(null, new());
        }

        public static SortedSet<FileRecord> GetFiles(bool showHidden)
        {
            CurrentDirectory.SetFileListingStrategy(showHidden);
            Dictionary<string, FileType> filesDict = CurrentDirectory.GetFiles();

            SortedSet<FileRecord> result = new(new FileRecordComparer());

            foreach (string filename in filesDict.Keys.Order())
            {
                FileType type = filesDict[filename];
                result.Add(new(filename, icons[type], type, ""));
            }

            if (addedFiles.TryGetValue(CurrentDirectory.Name, out List<FileRecord>? lst))
            {
                foreach (FileRecord rec in lst) result.Add(rec);
            }

            return result;
        }

        public static SortedSet<FileNode> GetFileNodes(bool showHidden)
        {
            CurrentDirectory.SetFileListingStrategy(showHidden);

            List<Node<FileSystemInfo>> nodes = CurrentDirectory.GetFileNodes();

            SortedSet<FileNode> set = new(new FileNodeComparer());
            foreach (Node<FileSystemInfo> node in nodes) set.Add(ConvertNode(node));

            if (addedFiles.TryGetValue(CurrentDirectory.Name, out List<FileRecord>? lst))
            {
                foreach (FileRecord rec in lst) set.Add(new(rec));  // because we can drag'n'drop only files, it won`t contain children
            }

            return set;
        }

        private static FileNode ConvertNode(Node<FileSystemInfo> node)
        {
            ObservableCollection<FileNode> subnodes = [];
            if (node.SubNodes.Count > 0)
            {
                foreach (Node<FileSystemInfo> subnode in node.SubNodes) subnodes.Add(ConvertNode(subnode));
            }

            string filename = node.Value.Name;
            FileType type = node.Value.GetFileType();
            string icon = icons[type];
            string fullPath = node.Value.FullName;
            FileRecord rec = new(filename, icon, type, fullPath);

            return subnodes.Count > 0 ? new(rec, subnodes) : new(rec);
        }

        public static event EventHandler? TempFileAdded;
    }
}
