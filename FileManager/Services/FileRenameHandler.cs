using FileManager.Infrastructure.Commands;
using System.Collections.Generic;
using System.IO;

namespace FileManager.Services
{
    public static class FileRenameHandler
    {
        /// <returns>New filenames</returns>
        public static string[] Rename(string[] oldFilenames, string newFilename)
        {
            if (oldFilenames.Length == 1)
            {
                Rename(oldFilenames[0], newFilename);
                return [newFilename];
            }
            else
            {
                List<string> newFilenames = [];
                for (int i = 0; i < oldFilenames.Length; i++)
                {
                    string filenameWithNumber = AddNumberToFilename(newFilename, i);
                    Rename(oldFilenames[i], filenameWithNumber);
                    newFilenames.Add(filenameWithNumber);
                }
                return [.. newFilenames];
            }
        }

        private static void Rename(string oldFilename, string newFilename)
        {
            if (!Path.IsPathFullyQualified(oldFilename)) oldFilename = Path.Combine(CurrentDirectory.Name, oldFilename);
            if (!Path.IsPathFullyQualified(newFilename)) newFilename = Path.Combine(CurrentDirectory.Name, newFilename);

            if (File.Exists(oldFilename)) DriveSyncHandler.Instance.RegisterCommand(new RenameFileCommand(oldFilename, newFilename));
        }

        private static string AddNumberToFilename(string filename, int number)
        {
            string extension = Path.GetExtension(filename);     // extension includes a period
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            return $"{filenameWithoutExtension}({number}){extension}";
        }
    }
}
