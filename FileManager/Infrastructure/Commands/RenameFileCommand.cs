using FileManager.Services;
using System;
using System.IO;

namespace FileManager.Infrastructure.Commands
{
    internal class RenameFileCommand(string oldFilename, string newFilename) : ICommand
    {
        public void Execute()
        {
            if (oldFilename == string.Empty || newFilename == string.Empty) DialogBoxes.ShowWarningBox("Empty filename");

            if (!File.Exists(oldFilename)) DialogBoxes.ShowWarningBox($"File {oldFilename} doesn`t exist");

            if (File.Exists(newFilename)) DialogBoxes.ShowWarningBox($"File {newFilename} already exists");

            File.Move(oldFilename, newFilename);
        }
    }
}
