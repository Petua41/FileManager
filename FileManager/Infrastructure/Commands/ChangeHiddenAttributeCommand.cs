using FileManager.Services;
using System.IO;
using System.Security;

namespace FileManager.Infrastructure.Commands
{
    internal class ChangeHiddenAttributeCommand(string filename, bool hidden) : ICommand
    {
        public void Execute()
        {
            FileSystemInfo fsInfo;
            if (File.Exists(filename)) fsInfo = new FileInfo(filename);
            else if (Directory.Exists(filename)) fsInfo = new DirectoryInfo(filename);
            else
            {
                DialogBoxes.ShowWarningBox($"Cannot make {filename} hidden: file doesn`t exist");
                return;
            }

            try 
            {
                if (hidden) fsInfo.Attributes |= FileAttributes.Hidden;     // set bit flag to true
                else fsInfo.Attributes &= ~FileAttributes.Hidden;           // set bit flag to false
            }
            catch (SecurityException) { DialogBoxes.ShowWarningBox($"Cannot rename {filename}: access denied"); }
        }
    }
}
