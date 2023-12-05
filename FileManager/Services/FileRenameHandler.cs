using FileManager.Infrastructure.Commands;
using System.IO;

namespace FileManager.Services
{
    public static class FileRenameHandler
    {
        public static void Rename(string[] oldFilenames, string newFilename)
        {
            if (oldFilenames.Length == 1) Rename(oldFilenames[0], newFilename);
            else
            {
                for (int i = 0; i < oldFilenames.Length; i++)
                {
                    Rename(oldFilenames[i], AddNumberToFilename(newFilename, i));
                }
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
