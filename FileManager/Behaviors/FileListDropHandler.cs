using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using FileManager.ViewModels;
using System.IO;
using System.Linq;

namespace FileManager.Behaviors
{
    internal class FileListDropHandler : DropHandlerBase
    {
        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return Validate(e, sourceContext, targetContext, true);
        }

        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            return Validate(e, sourceContext, targetContext, false);
        }
        private static bool Validate(DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute)
        {
            if (sourceContext is not null
                || targetContext is not EditViewModel model
                || !e.Data.Contains(DataFormats.Files)) return false;

            string? path = e.Data?.GetFiles()?.Select(f => f.Path.LocalPath)?.FirstOrDefault();

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return false;

            if (bExecute)
            {
                return EditViewModel.AcceptDragAndDrop(path);
            }

            return true;
        }
    }
}
