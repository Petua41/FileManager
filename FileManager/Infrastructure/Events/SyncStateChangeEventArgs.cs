using FileManager.Services;
using System;

namespace FileManager.Infrastructure.Events
{
    public class SyncStateChangeEventArgs(SyncState newState) : EventArgs
    {
        public SyncState NewState { get; } = newState;
    }
}
