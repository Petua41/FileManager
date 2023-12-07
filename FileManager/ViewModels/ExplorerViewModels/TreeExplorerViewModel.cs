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
        private const int MAX_ITERATIONS_COUNT = 100;

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

            return [.. nodes];
        }

        private async Task<ObservableCollection<FileNode>> GetFilesThroughCoroutine()
        {
            IsLoading = true;

            int iterations = 0;

            FileNode intermediateRoot = new();      // it`s not good to create empty FileNode, but Avalonia diplays it correctly

            foreach (FileNode nodeVersion in FilesCollectionConverter.GetCurrentDirectoryNode(ExplorerViewModel.ShouldShowHidden))
            {
                intermediateRoot = nodeVersion;
                iterations++;

                if (iterations > MAX_ITERATIONS_COUNT)
                {
                    break;      // here will be question like "Continue?"
                }
            }
            
            return [intermediateRoot];
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
