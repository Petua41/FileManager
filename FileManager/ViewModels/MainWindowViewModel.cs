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
            if (DriveSyncHandler.Instance.State == SyncState.NotSynced) await DriveSyncHandler.Instance.Sync();   // because of await, we don`t need to wait additionally
        }

        public ReactiveCommand<Unit, Unit> WindowClosed { get; }
    }
}
