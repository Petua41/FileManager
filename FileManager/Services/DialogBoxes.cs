using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace FileManager.Services
{
    public static class DialogBoxes
    {
        public static void ShowWarningBox(string message)
        {
            Dispatcher.UIThread.Invoke(() => ShowWarningBoxInternal(message));
        }

        private static void ShowWarningBoxInternal(string message)
        {
            IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard("Warning", message, ButtonEnum.Ok, Icon.Warning);
            box.ShowAsync();        // ShowAsunc shows popup, if can. Otherwise, as window
        }
    }
}
