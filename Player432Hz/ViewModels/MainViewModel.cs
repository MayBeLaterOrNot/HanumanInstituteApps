﻿using System.Linq;
using System.Reactive.Linq;
using Avalonia.Input;
using HanumanInstitute.Apps.AdRotator;
using HanumanInstitute.Avalonia;
using HanumanInstitute.MvvmDialogs;
using ReactiveUI;

namespace HanumanInstitute.Player432Hz.ViewModels;

/// <summary>
/// Represents the playlist editor.
/// </summary>
public class MainViewModel : MainViewModelBase<AppSettingsData>
{
    private readonly IPlaylistViewModelFactory _playlistFactory;
    private readonly IFilesListViewModel _filesListViewModel;
    private readonly IDialogService _dialogService;
    private readonly IPathFixer _pathFixer;

    public MainViewModel(ISettingsProvider<AppSettingsData> settings, IAppUpdateService appUpdateService, IPlaylistViewModelFactory playlistFactory,
        IFilesListViewModel filesListViewModel, IDialogService dialogService, IPathFixer pathFixer, IEnvironmentService environment, IAdRotatorViewModel adRotator) : 
        base(settings, appUpdateService, environment)
    {
        _playlistFactory = playlistFactory;
        _playlistFactory.OwnerViewModel = this;
        _filesListViewModel = filesListViewModel;
        _dialogService = dialogService;
        _pathFixer = pathFixer;
        AdRotator = adRotator;

        Playlists.WhenAnyValue(x => x.CurrentItem).Subscribe(_ => Playlists_CurrentChanged());
        // ConvertFromSettings();
    }
    
    public IAdRotatorViewModel AdRotator { get; }

    public override async void OnLoaded()
    {
        base.OnLoaded();
        await PromptFixPathsAsync().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Prompts to fix invalid paths, if any.
    /// </summary>
    public async Task PromptFixPathsAsync()
    {
        var changed = await _pathFixer.ScanAndFixFoldersAsync(this, 
            Settings.Playlists.Select(x => (FixFolderItem)new FixFolder<string>(x.Folders)).ToList()).ConfigureAwait(false);
        if (changed)
        {
            ConvertFromSettings();
            _settings.Save();
        }
    }

    /// <summary>
    /// Returns the list of playlists with selection properties that can be bound to the UI.
    /// </summary>
    public ICollectionView<IPlaylistViewModel> Playlists { get; } = new CollectionView<IPlaylistViewModel>();

    public ReactiveCommand<TappedEventArgs, Unit> StartPlayList => _startPlayList ??= ReactiveCommand.Create<TappedEventArgs>(StartPlayListImpl);
    private ReactiveCommand<TappedEventArgs, Unit>? _startPlayList;
    private void StartPlayListImpl(TappedEventArgs e)
    {
        _filesListViewModel.Files.CurrentPosition = -1;
        _filesListViewModel.Play.Execute().Subscribe();
    }

    public ReactiveCommand<TappedEventArgs, Unit> StartPlayFile => _startPlayFile ??= ReactiveCommand.Create<TappedEventArgs>(StartPlayFileImpl);
    private ReactiveCommand<TappedEventArgs, Unit>? _startPlayFile;
    private void StartPlayFileImpl(TappedEventArgs e) => _filesListViewModel.Play.Execute().Subscribe();

    /// <summary>
    /// Adds a new playlist to the list.
    /// </summary>
    public RxCommandUnit AddPlaylist => _addPlaylist ??= ReactiveCommand.Create(AddPlaylistImpl);
    private RxCommandUnit? _addPlaylist;
    private void AddPlaylistImpl()
    {
        var newPlaylist = _playlistFactory.Create();
        Playlists.Source.Add(newPlaylist);
        Playlists.MoveCurrentToLast();
    }

    /// <summary>
    /// Deletes selected playlist from the list.
    /// </summary>
    public RxCommandUnit DeletePlaylist => _deletePlaylist ??= ReactiveCommand.Create(DeletePlaylistImpl,
        this.WhenAnyValue(x => x.Playlists.CurrentItem).Select(x => x != null));
    private RxCommandUnit? _deletePlaylist;
    private void DeletePlaylistImpl()
    {
        if (Playlists.CurrentPosition > -1)
        {
            Playlists.Source.RemoveAt(Playlists.CurrentPosition);
        }
    }

    /// <summary>
    /// When a playlist is selected, display the files.
    /// </summary>
    private void Playlists_CurrentChanged()
    {
        _filesListViewModel.SetPaths(Playlists.CurrentItem?.Folders.Source);
    }

    /// <inheritdoc />
    protected override void ConvertFromSettings()
    {
        var playlists = _settings.Value.Playlists;
        Playlists.Source.Clear();
        Playlists.Source.AddRange(playlists.Select(x => _playlistFactory.Create(x)));
    }

    /// <inheritdoc />
    protected override void ConvertToSettings()
    {
        _settings.Value.Playlists.Clear();
        _settings.Value.Playlists.AddRange(Playlists.Source.Select(x =>
            new SettingsPlaylistItem(x.Name, x.Folders.Source)));
    }

    /// <inheritdoc />
    protected override Task ShowAboutImplAsync() => _dialogService.ShowAboutAsync(this);

    /// <inheritdoc />
    protected override Task ShowSettingsImplAsync() => _dialogService.ShowSettingsAsync(this, _settings.Value);
}
