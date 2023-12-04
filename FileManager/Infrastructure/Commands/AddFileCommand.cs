using FileManager.Services;
using System;
using System.IO;

namespace FileManager.Infrastructure.Commands
{
    internal class AddFileCommand(string filename, string directory) : ICommand
    {
        public void Execute()
        {
            if (!File.Exists(filename)) DialogBoxes.ShowWarningBox($"File {filename} not found!");

            string name = Path.GetFileName(filename);
            string newFilename = Path.Combine(directory, name);
            if (File.Exists(newFilename) || Directory.Exists(newFilename)) DialogBoxes.ShowWarningBox($"{newFilename} already exists!");

            try { File.Copy(filename, newFilename, false); }
            catch (UnauthorizedAccessException) { DialogBoxes.ShowWarningBox($"Cannot paste {newFilename}: access denied"); }
        }
    }
}
