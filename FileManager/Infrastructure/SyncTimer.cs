using Avalonia.Threading;
using FileManager.Services;
using System;

namespace FileManager.Infrastructure
{
    internal class SyncTimer
    {
        private readonly TimeSpan SYNC_INTERVAL = new(0, 0, 30);     // 30 seconds

        private readonly DispatcherTimer timer;

        public SyncTimer()
        {
            timer = new()
            {
                Interval = SYNC_INTERVAL
            };
            timer.Tick += OnTimerTick;
        }

        public void Initialize()
        {
            timer.Start();
        }

        private async void OnTimerTick(object? sender, EventArgs e)
        {
            bool wasSynced = await DriveSyncHandler.Sync();
            if (wasSynced) LastSync = DateTime.Now;
            LastSyncChanged?.Invoke(null, new());
        }

        public DateTime? LastSync { get; private set; }

        public event EventHandler? LastSyncChanged;
    }
}
