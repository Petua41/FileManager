using FileManager.Infrastructure.Commands;
using FileManager.Infrastructure.Converters;
using FileManager.Models;
using FileManager.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
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

            if (!File.Exists(newPath)) return false;

            try
            {
                using FileStream fs = File.Create(newPath, 0, FileOptions.DeleteOnClose);
            }
            catch { return false; }     // if any exception occurs, we cannot paste file

            DriveSyncHandler.Instance.RegisterCommand(new AddFileCommand(path, CurrentDirectory.Name));
            FilesCollectionConverter.AddFile(path);

            return true;
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
                HiddenAttributeChanger.ChangeAttribute([file.Filename], shouldBeHidden);
            }
            else
            {
                string[] filenames = SelectedItems.Select(rec => rec.Filename).ToArray();

                FileRenameHandler.Rename(filenames, newFilename);
                HiddenAttributeChanger.ChangeAttribute(filenames, shouldBeHidden);
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
            get => selectedHiddenIndex;       // binded property should contain getter
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
