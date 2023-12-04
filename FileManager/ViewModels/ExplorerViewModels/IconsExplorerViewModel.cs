using FileManager.Infrastructure.Converters;
using FileManager.Infrastructure.Extensions;
using FileManager.Models;
using FileManager.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace FileManager.ViewModels.ExplorerViewModels
{
    public class IconsExplorerViewModel : ViewModelBase
    {
        private bool isLoading;

        public IconsExplorerViewModel()
        {
            ExplorerViewModel.ShouldUpdateFilesChanged += (sender, e) =>
            {
                if (e.NewValue) this.RaisePropertyChanged(nameof(Files));
            };
        }

        private async Task<ObservableCollection<FileRecord>> GetFiles()
        {
            IsLoading = true;

            SortedSet<FileRecord> files = await Task.Run(() => FilesCollectionConverter.GetFiles(ExplorerViewModel.ShouldShowHidden));

            IsLoading = false;
            return new(files);
        }

        public Task<ObservableCollection<FileRecord>> Files => GetFiles();

        public static FileRecord? SelectedItem
        {
            get => null;
            set
            {
                if (value is not null && value.Value.Type == FileType.Directory)
                {
                    string filePath = string.IsNullOrWhiteSpace(value.Value.FullPath)
                        ? Path.Combine(CurrentDirectory.Name, value.Value.Filename)
                        : value.Value.FullPath;

                    if (!Directory.Exists(filePath)) DialogBoxes.ShowWarningBox($"Directory {filePath} doesn`t exist!");
                    else
                    {
                        CurrentDirectory.Go(filePath);
                    }
                }
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set => this.RaiseAndSetIfChanged(ref isLoading, value);
        }
    }
}
