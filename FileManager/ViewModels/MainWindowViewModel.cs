using FileManager.Services;
using ReactiveUI;
using System.Reactive;

namespace FileManager.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            WindowClosed = ReactiveCommand.Create(OnWindowClosed);
        }

        private async void OnWindowClosed()
        {
            if (DriveSyncHandler.State == SyncState.NotSynced) await DriveSyncHandler.Sync();   // because of await, we don`t nned to wait additionally
        }

        public ReactiveCommand<Unit, Unit> WindowClosed { get; }
    }
}
