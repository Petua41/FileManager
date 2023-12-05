using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using FileManager.Infrastructure;
using FileManager.Models;
using FileManager.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;

namespace FileManager.ViewModels;

public class MainViewModel : ViewModelBase
{
    private static readonly Dictionary<SyncState, string> syncStateIcons = new()
    {
        { SyncState.Syncing, "fa-sync" }, { SyncState.Synced, "fa-check-circle" }, { SyncState.NotSynced, "fa-circle-xmark" }
    };

    private static readonly Dictionary<SyncState, string> syncStateAnimations = new()
    {
        { SyncState.Syncing, "Spin" }, { SyncState.Synced, "None" }, { SyncState.NotSynced, "None" }
    };

    private SyncState currentState = DriveSyncHandler.Instance.State;
    private readonly SyncTimer timer;

    public MainViewModel()
    {
        timer = new();
        timer.Initialize();

        DriveSyncHandler.Instance.StateChanged += (sender, e) => CurrentState = DriveSyncHandler.Instance.State;
        timer.LastSyncChanged += (sender, e) => this.RaisePropertyChanged(nameof(LastSync));
        CurrentDirectory.CurrentDirectorySwitched += OnCurrentDirectorySwitched;

        CloseClicked = ReactiveCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) lifetime.Shutdown();
        });
    }

    private void OnCurrentDirectorySwitched(object? sender, EventArgs e)
    {
        this.RaisePropertyChanged(nameof(Recent));
    }

    public string? LastSync
    {
        get
        {
            DateTime? lastSync = timer.LastSync;
            return lastSync?.ToString("H:m:ss");
        }
    }

    public static ObservableCollection<RecentDirectoryRecord> Recent => new(CurrentDirectory.Recent
      .Select(dir => new RecentDirectoryRecord(dir.FullName, dir.Name)));

    public static int SelectedRecentIndex
    {
        get => -1;
        set
        {
            if (value < 0) return;

            RecentDirectoryRecord rec = Recent[value];

            if (rec.FullName == CurrentDirectory.Name) return;

            if (Directory.Exists(rec.FullName)) CurrentDirectory.Go(rec.FullName);
            else
            {
                DialogBoxes.ShowWarningBox($"Directory {rec.Name} doesn`t exist");
            }
        }
    }

    public string SyncIcon => syncStateIcons[CurrentState];

    public string SyncAnimation => syncStateAnimations[CurrentState];

    public ReactiveCommand<Unit, Unit> CloseClicked { get; }

    private SyncState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            this.RaisePropertyChanged(nameof(SyncIcon));
            this.RaisePropertyChanged(nameof(SyncAnimation));
        }
    }
}