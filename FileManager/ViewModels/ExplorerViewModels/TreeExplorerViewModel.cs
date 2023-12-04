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
    internal class TreeExplorerViewModel : ViewModelBase
    {
        private bool isLoading = true;

        public TreeExplorerViewModel()
        {
            ExplorerViewModel.HiddenChanged += (sender, e) => this.RaisePropertyChanged(nameof(FileNodes));
            ExplorerViewModel.ShouldUpdateFilesChanged += (sender, e) =>
            {
                if (e.NewValue) this.RaisePropertyChanged(nameof(FileNodes));
            };
        }

        private async Task<ObservableCollection<FileNode>> GetFiles()
        {
            IsLoading = true;

            SortedSet<FileNode> nodes = await Task.Run(() => FilesCollectionConverter.GetFileNodes(ExplorerViewModel.ShouldShowHidden));

            IsLoading = false;

            return new(nodes);
        }

        public Task<ObservableCollection<FileNode>> FileNodes => GetFiles();

        public bool IsLoading
        {
            get => isLoading;
            set => this.RaiseAndSetIfChanged(ref isLoading, value);
        }

        public static FileNode? SelectedItem
        {
            get => null;
            set
            {
                if (value is not null && value.Value.File.Type == FileType.Directory)
                {
                    string filePath = string.IsNullOrWhiteSpace(value.Value.File.FullPath)
                        ? Path.Combine(CurrentDirectory.Name, value.Value.File.Filename)
                        : value.Value.File.FullPath;

                    if (!Directory.Exists(filePath)) DialogBoxes.ShowWarningBox($"Directory {filePath} doesn`t exist!");
                    else
                    {
                        CurrentDirectory.Go(filePath);
                    }
                }
            }
        }
    }
}
