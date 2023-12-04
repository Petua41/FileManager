using System.Collections.Generic;
using System.IO;

namespace FileManager.Infrastructure.Extensions
{
    public enum FileType { Unknown, Directory, Image, Text }

    internal static class FileSystemExtensions
    {
        private static readonly List<string> textExtensions = [".txt", ".rtf", ".md", ".xml"];
        private static readonly List<string> imageExtensions = [".png", ".jpg", ".svg"];

        public static FileType GetFileType(this FileSystemInfo fileSystem)
        {
            if (fileSystem is DirectoryInfo) return FileType.Directory;

            string extension = fileSystem.Extension;
            if (textExtensions.Contains(extension)) return FileType.Text;
            if (imageExtensions.Contains(extension)) return FileType.Image;

            return FileType.Unknown;
        }

        public static bool IsHidden(this FileSystemInfo fileSystem)
        {
            return fileSystem.Attributes.HasFlag(FileAttributes.Hidden);
        }
    }
}
