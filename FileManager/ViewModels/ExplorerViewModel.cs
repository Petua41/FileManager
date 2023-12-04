using FileManager.Infrastructure.Events;
using FileManager.Services;
using ReactiveUI;
using System;
using System.Reactive;

namespace FileManager.ViewModels
{
    internal class ExplorerViewModel : ViewModelBase
    {
        private static bool shouldUpdateFiles = true;
        private bool treeMode = false;

        public ExplorerViewModel()
        {
            UpClicked = ReactiveCommand.Create(OnUpClicked);
            CurrentDirectory.CurrentDirectorySwitched += (sender, e) => ShouldUpdateFiles = true;
        }

        private void OnUpClicked()
        {
            if (CurrentDirectory.IsRootDirectory()) return;

            CurrentDirectory.GoUp();
            ShouldUpdateFiles = true;
        }

        public static bool ShouldShowHidden { get; set; } = false;  // this property is for IconsExplorerViewModel, TreeExplorerViewModel and EditViewModel

        public bool ShowHidden  // this property is for bindings
        {
            get => ShouldShowHidden;
            set
            {
                ShouldShowHidden = value;
                ShouldUpdateFiles = true;
                this.RaisePropertyChanged(nameof(ShowHidden));
                HiddenChanged?.Invoke(null, new(ShouldShowHidden));
            }
        }

        public int CarouselSelectedIndex
        {
            get => TreeMode ? 1 : 0;
        }

        public bool TreeMode
        {
            get => treeMode;
            set
            {
                treeMode = value;
                this.RaisePropertyChanged(nameof(TreeMode));
                this.RaisePropertyChanged(nameof(CarouselSelectedIndex));
            }
        }

        private static bool ShouldUpdateFiles
        {
            set
            {
                shouldUpdateFiles = value;
                ShouldUpdateFilesChanged?.Invoke(null, new(shouldUpdateFiles));
            }
        }

        public ReactiveCommand<Unit, Unit> UpClicked { get; }

        public static event EventHandler<BoolValueChangedEventArgs>? HiddenChanged;

        public static event EventHandler<BoolValueChangedEventArgs>? ShouldUpdateFilesChanged;
    }
}
