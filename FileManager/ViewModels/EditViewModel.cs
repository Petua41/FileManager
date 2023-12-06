using FileManager.Infrastructure.Commands;
using FileManager.Infrastructure.Converters;
using FileManager.Models;
using FileManager.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FileManager.ViewModels
{
    internal class EditViewModel : ViewModelBase
    {
        bool shouldBeHidden = false;
        int selectedHiddenIndex = 0;
        private bool isLoading;

        public EditViewModel()
        {
            ApplyClicked = ReactiveCommand.Create(OnApplyClicked);

            CurrentDirectory.CurrentDirectorySwitched += (sender, e) => this.RaisePropertyChanged(nameof(Files));
            FilesCollectionConverter.TempFileAdded += (sender, e) => this.RaisePropertyChanged(nameof(Files));
            ExplorerViewModel.HiddenChanged += (sender, e) => this.RaisePropertyChanged(nameof(Files));
        }

        public static bool AcceptDragAndDrop(string path)
        {
            string newPath = Path.Combine(CurrentDirectory.Name, Path.GetFileName(path));

            if (File.Exists(newPath)) return false;     // file already exists

            if (!CheckDirectoryAccess(CurrentDirectory.Name)) return false;

            DriveSyncHandler.Instance.RegisterCommand(new AddFileCommand(path, CurrentDirectory.Name));
            FilesCollectionConverter.AddFile(path);

            return true;
        }

        private static bool CheckDirectoryAccess(string directoryPath)
        {
            if (OperatingSystem.IsWindows()) return CheckDirectoryAccessOnWindows(directoryPath);
            else return CheckDirectoryAccessOnOtherSystems(directoryPath);
        }

        [SupportedOSPlatform("windows")]
        private static bool CheckDirectoryAccessOnWindows(string directoryPath)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            SecurityIdentifier? secID = identity.User;
            if (secID is null) return false;        // we are not a user. We cannot write
            string userSID = secID.Value;
            string userName = identity.Name;

            DirectorySecurity security = new DirectoryInfo(directoryPath).GetAccessControl();
            AuthorizationRuleCollection rules = security.GetAccessRules(includeExplicit: true, includeInherited: true, typeof(NTAccount));

            foreach (FileSystemAccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == userName                            // in some cases IdentityReference.Value is username
                    || rule.IdentityReference.Value == userSID                          // and, in some cases, user`s SDDL (stored in userSID)
                    || (identity.Groups?.Contains(rule.IdentityReference) ?? false))    // if we don`t belong to any group, our group cannot write anything
                {
                    return rule.FileSystemRights.HasFlag(FileSystemRights.CreateFiles)
                        && rule.AccessControlType == AccessControlType.Allow;
                }
            }

            return false;   // no rules for current user
        }

        private static bool CheckDirectoryAccessOnOtherSystems(string directoryPath)
        {
            try
            {
                using FileStream fs = File.Create(Path.Combine(directoryPath, "tempFile"), 0, FileOptions.DeleteOnClose);
                return true;
            }
            catch { return false; }     // if any exception occurs, we cannot paste file
        }

        private void OnApplyClicked()
        {
            string newFilename = InputFilename.Trim();

            if (newFilename.Length == 0) return;

            if (SelectedItems.Count == 0) return;
            if (SelectedItems.Count == 1)
            {
                FileRecord file = SelectedItems[0];
                if (file.Filename != newFilename) FileRenameHandler.Rename([file.Filename], newFilename);
                HiddenAttributeChanger.ChangeAttribute([newFilename], shouldBeHidden);      // we rename file and THEN change attribute, so we need to cahnge attribute on new filename
            }
            else
            {
                string[] filenames = SelectedItems.Select(rec => rec.Filename).ToArray();

                string[] newFilenames = FileRenameHandler.Rename(filenames, newFilename);
                HiddenAttributeChanger.ChangeAttribute(newFilenames, shouldBeHidden);
            }
        }

        private async Task<ObservableCollection<FileRecord>> GetFiles()
        {
            IsLoading = true;

            SortedSet<FileRecord> files = await Task.Run(() => FilesCollectionConverter.GetFiles(ExplorerViewModel.ShouldShowHidden));

            IsLoading = false;
            return new(files);
        }

        public Task<ObservableCollection<FileRecord>> Files => GetFiles();

        public string InputFilename { get; set; } = string.Empty;

        public int SelcetedHiddenIndex
        {
            get => selectedHiddenIndex;       // bound property should contain getter
            set
            {
                shouldBeHidden = value == 1;    // 1 is hidden
                selectedHiddenIndex = value;
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set => this.RaiseAndSetIfChanged(ref isLoading, value);
        }

        public List<FileRecord> SelectedItems { get; set; } = [];

        public ReactiveCommand<Unit, Unit> ApplyClicked { get; }
    }
}
