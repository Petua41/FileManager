using FileManager.Infrastructure.Commands;
using FileManager.Infrastructure.Events;
using FileManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FileManager.Services
{
    public enum SyncState { NotSynced, Syncing, Synced }

    public static class DriveSyncHandler
    {
        private static readonly List<ICommand> pendingChanges = [];

        public static void RegisterCommand(ICommand command)
        {
            pendingChanges.Add(command);

            if (State == SyncState.Synced)
            {
                State = SyncState.NotSynced;
                RaiseStateChangedEvent();
            }
        }

        public static async Task<bool> Sync()
        {
            bool synced = false;

            if (NeedsSync)
            {
                State = SyncState.Syncing;
                RaiseStateChangedEvent();

                await Task.Run(WriteToDiskAndWait);
                // In paid version, uncomment this line and comment previous
                // await Task.Run(WriteToDisk);

                synced = true;
            }

            if (State != SyncState.Synced)
            {
                State = SyncState.Synced;
                RaiseStateChangedEvent();
            }

            return synced;
        }

        private static void RaiseStateChangedEvent()   // I don`t know if I should do this
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised
            EventHandler<SyncStateChangeEventArgs>? eventCopy = StateChanged;

            if (eventCopy is not null) eventCopy(null, new(State));      // we cannot use "this" as sender. This class is static
        }

        private static void WriteToDiskAndWait()
        {
            Stopwatch sw = Stopwatch.StartNew();

            WriteToDisk();

            sw.Stop();

            if (sw.ElapsedMilliseconds < 1000) Thread.Sleep(1500 - (int)sw.ElapsedMilliseconds);    // If syncing is too fast, artifitially slow it
        }

        private static void WriteToDisk()
        {
            foreach (ICommand command in pendingChanges)
            {
                command.Execute();
            }

            pendingChanges.Clear();
        }

        public static SyncState State { get; private set; } = SyncState.Synced;

        private static bool NeedsSync => State != SyncState.Syncing && pendingChanges.Count > 0;

        public static event EventHandler<SyncStateChangeEventArgs>? StateChanged;
    }
}
