using FileManager.Infrastructure.Commands;
using FileManager.Infrastructure.Extensions;
using System.IO;

namespace FileManager.Services
{
    public static class HiddenAttributeChanger
    {
        public static void ChangeAttribute(string[] filenames, bool shouldBeHidden)
        {
            foreach (string filename in filenames) ChangeAttribute(filename, shouldBeHidden);
        }

        private static void ChangeAttribute(string filename, bool shouldBeHidden)
        {
            FileSystemInfo fsInfo;

            if (!Path.IsPathFullyQualified(filename)) filename = Path.Combine(CurrentDirectory.Name, filename);

            if (File.Exists(filename)) fsInfo = new FileInfo(filename);
            else if (Directory.Exists(filename)) fsInfo = new DirectoryInfo(filename);
            else
            {
                DialogBoxes.ShowWarningBox($"Cannot make {filename} hidden: file doesn`t exist");
                return;
            }

            if (fsInfo.IsHidden() != shouldBeHidden)
            {
                DriveSyncHandler.Instance.RegisterCommand(new ChangeHiddenAttributeCommand(filename, shouldBeHidden));
            }
        }
    }
}
