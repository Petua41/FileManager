using FileManager.Infrastructure.Converters;
using FileManager.Infrastructure.Extensions;
using FileManager.Models;
using FileManager.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FileManager.ViewModels.ExplorerViewModels
{
    internal class TreeExplorerViewModel : ViewModelBase
    {
        private const int MAX_WAIT_MILLISECONDS = 5_000;       // 5 seconds

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

            FileNode intermediateRoot = new();      // it`s not good to create empty FileNode, but Avalonia diplays it correctly

            int tempMaxWaitMilliseconds = MAX_WAIT_MILLISECONDS;

            Stopwatch stopwatch = Stopwatch.StartNew();

            await foreach (FileNode nodeVersion in FilesCollectionConverter.GetCurrentDirectoryNode(ExplorerViewModel.ShouldShowHidden))
            {
                intermediateRoot = nodeVersion;

                if (stopwatch.ElapsedMilliseconds > tempMaxWaitMilliseconds)
                {
                    bool answer = await DialogBoxes.AskQuestion("Looks like there are a lot of files. Continue or show partial result?",
                        "Continue", "Show result");
                    if (answer)     // yes is continue
                    {
                        tempMaxWaitMilliseconds *= 2;       // so that questions will be asked less and less often
                        stopwatch.Restart();
                    }
                    else break;
                }
            }

            IsLoading = false;

            return [intermediateRoot];
        }

        public Task<ObservableCollection<FileNode>> FileNodes => MainViewModel.UseCoroutine
            ? GetFilesThroughCoroutine()
            : GetFiles();

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
