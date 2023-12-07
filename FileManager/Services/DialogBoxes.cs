using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using System.Collections.Generic;
using Avalonia.Controls;
using System.Threading.Tasks;

namespace FileManager.Services
{
    public static class DialogBoxes
    {
        /// <summary>
        /// Shows message box with "exclamation mark" icon and "warning" in the title
        /// </summary>
        /// <param name="message">Warning text to be displayed</param>
        public static void ShowWarningBox(string message)
        {
            Dispatcher.UIThread.Invoke(() => ShowWarningBoxInternal(message));      // default priority is Send, which is above Normal
        }

        private static void ShowWarningBoxInternal(string message)
        {
            IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard("Warning", message, ButtonEnum.Ok, Icon.Warning);
            box.ShowAsync();        // ShowAsunc shows popup, if can. Otherwise, as window
        }

        /// <summary>
        /// Asks question with two possible answers
        /// </summary>
        /// <param name="question">Question to be asked</param>
        /// <param name="yesText">Text on the button, that is considered as "yes"</param>
        /// <param name="noText">Text on the button, that is considered as "no"</param>
        /// <returns>true if user pressed "yes", false otherwise</returns>
        public async static Task<bool> AskQuestion(string question, string yesText="Yes", string noText="No")
        {
            return await Dispatcher.UIThread.InvokeAsync(() => AskQuestionInternal(question, yesText, noText));
        }

        private async static Task<bool> AskQuestionInternal(string question, string yesText, string noText)
        {
            MessageBoxCustomParams parameters = new()
            {
                ButtonDefinitions = new List<ButtonDefinition>()
                {
                    new() { Name=yesText }, new() { Name=noText }
                },
                ContentMessage = question,
                Icon = Icon.Question,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Topmost = true,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            IMsBox<string> box = MessageBoxManager.GetMessageBoxCustom(parameters);
            string result = await box.ShowAsync();

            return result == yesText;
        }
    }
}
