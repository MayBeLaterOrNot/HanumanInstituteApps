﻿using System.Linq;
using System.Linq.Expressions;
using DynamicData;
using HanumanInstitute.Apps.AdRotator;
using HanumanInstitute.Avalonia;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ReactiveUI;

namespace HanumanInstitute.Converter432Hz.ViewModels;

/// <summary>
/// Represents the main application window.
/// </summary>
public class MainViewModel : MainViewModelBase<AppSettingsData>
{
    private readonly IDialogService _dialogService;
    private readonly IFileSystemService _fileSystem;
    private readonly IFileLocator _fileLocator;
    private readonly IAppPathService _appPath;
    private readonly IEnvironmentService _environment;
    private readonly IPitchDetector _pitchDetector;
    
    public AppSettingsData AppSettings => _settings.Value;

    public MainViewModel(ISettingsProvider<AppSettingsData> settings, IAppUpdateService appUpdateService, IEncoderService encoder,
        IDialogService dialogService, IFileSystemService fileSystem, IFileLocator fileLocator, IAppPathService appPath, 
        IEnvironmentService environment, IPitchDetector pitchDetector, IAdRotatorViewModel adRotator) :
        base(settings, appUpdateService, environment)
    {
        Encoder = encoder;
        Encoder.Owner = this;
        _dialogService = dialogService;
        _fileSystem = fileSystem;
        _fileLocator = fileLocator;
        _appPath = appPath;
        _environment = environment;
        _pitchDetector = pitchDetector;
        AdRotator = adRotator;

        Bind(x => x.Settings.Encode.Format, x => x.FormatsList.SelectedValue);
        Bind(x => x.Settings.Encode.Bitrate, x => x.BitrateList.SelectedValue);
        Bind(x => x.Settings.Encode.BitsPerSample, x => x.BitsPerSampleList.SelectedValue);
        Bind(x => x.Settings.Encode.SampleRate, x => x.SampleRateList.SelectedValue);
        Bind(x => x.Settings.FileExistsAction, x => x.FileExistsActionList.SelectedValue);
        
        _isBitrateVisible = this.WhenAnyValue(x => x.FormatsList.SelectedValue, x => x != EncodeFormat.Flac && x != EncodeFormat.Wav)
            .ToProperty(this, x => x.IsBitrateVisible);
        _isToggleBitrateVisible = this.WhenAnyValue(x => x.FormatsList.SelectedValue, x => x == EncodeFormat.Mp3)
            .ToProperty(this, x => x.IsToggleBitrateVisible);
        _isBitsPerSampleVisible = this.WhenAnyValue(x => x.FormatsList.SelectedValue, x => x == EncodeFormat.Flac || x == EncodeFormat.Wav)
            .ToProperty(this, x => x.IsBitsPerSampleVisible);
        _isQualitySpeedVisible = this.WhenAnyValue(x => x.FormatsList.SelectedValue, x => x == EncodeFormat.Mp3 || x == EncodeFormat.Flac)
            .ToProperty(this, x => x.IsQualitySpeedVisible);
        this.WhenAnyValue(x => x.Settings.Encode.Format).Subscribe(FillSampleRateList);

        // _completedString = this.WhenAnyValue(x => x.FilesLeft, x => x.CompletedCount,
        //     (left, completed) => left > 0 || completed > 0 ?
        //         $"({completed} / {completed + left})" : string.Empty)
        //     .ToProperty(this, x => x.CompletedString);

        // Encoder.FileCompleted += (_, _) =>
        // {
        //     CalcFilesLeft();
        //     CalcCompleted();
        // };
    }

    public IAdRotatorViewModel AdRotator { get; }

    private void Bind<T1, T2>(Expression<Func<MainViewModel, T1?>> expr1, Expression<Func<MainViewModel, T2?>> expr2)
    {
        this.WhenAnyValue(expr1)
            .BindTo(this, expr2);
        this.WhenAnyValue(expr2)
            .BindTo(this, expr1);
    }

    // /// <summary>
    // /// Gets the quantity of files left.
    // /// </summary>
    // [Reactive]
    // public int FilesLeft { get; private set; }
    //
    // /// <summary>
    // /// Gets the quantity of files completed.
    // /// </summary>
    // [Reactive]
    // public int CompletedCount { get; private set; }
    //
    // /// <summary>
    // /// Returns a string with "(CompletedCount / CompletedCount+FilesLeft)".
    // /// </summary>
    // public string CompletedString => _completedString.Value;
    // private readonly ObservableAsPropertyHelper<string> _completedString;

    /// <summary>
    /// Gets whether Bitrate control should be visible.
    /// </summary>
    public bool IsBitrateVisible => _isBitrateVisible.Value;
    private readonly ObservableAsPropertyHelper<bool> _isBitrateVisible;

    /// <summary>
    /// Gets or sets whether Toggle Bitrate button should be visible. 
    /// </summary>
    public bool IsToggleBitrateVisible => _isToggleBitrateVisible.Value;
    private readonly ObservableAsPropertyHelper<bool> _isToggleBitrateVisible;

    /// <summary>
    /// Gets whether Bits Per Sample control should be visible.
    /// </summary>
    public bool IsBitsPerSampleVisible => _isBitsPerSampleVisible.Value;
    private readonly ObservableAsPropertyHelper<bool> _isBitsPerSampleVisible;

    /// <summary>
    /// Gets whether QualitySpeed slider should be visible.
    /// </summary>
    public bool IsQualitySpeedVisible => _isQualitySpeedVisible.Value;
    private readonly ObservableAsPropertyHelper<bool> _isQualitySpeedVisible;

    /// <summary>
    /// The encoder service.
    /// </summary>
    public IEncoderService Encoder { get; }

    [Reactive]
    public int SourcesSelectedIndex { get; set; } = -1;

    public RxCommandUnit AddFiles => _addFiles ??= ReactiveCommand.CreateFromTask(AddFilesImplAsync);
    private RxCommandUnit? _addFiles;
    private async Task AddFilesImplAsync()
    {
        // throw new NotSupportedException();
        var settings = new OpenFileDialogSettings()
        {
            Title = "Select files to convert",
            Filters = new List<FileFilter>()
            {
                new("Audio files", _appPath.AudioExtensions),
                new("All files", "*")
            }
        };
        var files = await _dialogService.ShowOpenFilesDialogAsync(this, settings).ConfigureAwait(true);
        var validFiles = files.Where(x => _fileSystem.File.Exists(x.LocalPath)).ToList();

        var items = validFiles.Select(x => new FileItem(x.LocalPath, _fileSystem.Path.GetFileName(x.LocalPath))).ToList();
        ListExtensions.AddRange(Encoder.Sources, items);
        //CalcFilesLeft();

        // Auto-detect pitch.
        await items.ForEachAsync(async x =>
        {
            try
            {
                x.Pitch = await _pitchDetector.GetPitchAsync(x.Path).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }).ConfigureAwait(false);
    }

    public RxCommandUnit AddFolder => _addFolder ??= ReactiveCommand.CreateFromTask(AddFolderImplAsync);
    private RxCommandUnit? _addFolder;
    private async Task AddFolderImplAsync()
    {
        var settings = new OpenFolderDialogSettings() { Title = "Convert all audio files in folder" };
        var folder = await _dialogService.ShowOpenFolderDialogAsync(this, settings).ConfigureAwait(true);
        if (folder != null)
        {
            var folderName = _fileSystem.Path.GetFileName(folder.LocalPath.TrimEnd(_environment.DirectorySeparatorChar));
            var folderItem = new FolderItem(folder.LocalPath, _environment.DirectorySeparatorChar + folderName);
            var files = _fileLocator.GetAudioFiles(folder.LocalPath);
            folderItem.Files.AddRange(files.Select(x => new FileItem(x.Path, _fileSystem.Path.Combine(folderName, x.RelativePath))));
            Encoder.Sources.Add(folderItem);
            //CalcFilesLeft();
        }
    }

    public RxCommandUnit RemoveFile => _removeFile ??= ReactiveCommand.Create(RemoveFileImpl,
        this.WhenAnyValue(x => x.SourcesSelectedIndex, index => index > -1));
    private RxCommandUnit? _removeFile;
    private void RemoveFileImpl()
    {
        if (SourcesSelectedIndex > -1 && SourcesSelectedIndex < Encoder.Sources.Count)
        {
            var sel = SourcesSelectedIndex;
            Encoder.Sources.RemoveAt(sel);
            SourcesSelectedIndex = -1;
            SourcesSelectedIndex = sel >= Encoder.Sources.Count ? Encoder.Sources.Count - 1 : sel;
            //CalcFilesLeft();
        }
    }

    public RxCommandUnit BrowseDestination => _browseDestination ??= ReactiveCommand.CreateFromTask(BrowseDestinationImplAsync);
    private RxCommandUnit? _browseDestination;
    private async Task BrowseDestinationImplAsync()
    {
        var settings = new OpenFolderDialogSettings() { Title = "Destination" };
        var folder = await _dialogService.ShowOpenFolderDialogAsync(this, settings).ConfigureAwait(true);
        if (folder != null)
        {
            Settings.Destination = folder.LocalPath;
        }
    }

    // private void CalcFilesLeft() => FilesLeft =
    //     Encoder.Sources.Select(x => x switch
    //     {
    //         FolderItem folder => folder.Files.Count,
    //         _ => 1
    //     }).Sum(x => x);
    //
    // private void CalcCompleted() => CompletedCount =
    //     Encoder.ProcessingFiles.Count(x => x.Status == EncodeStatus.Completed);

    public ListItemCollectionView<EncodeFormat> FormatsList { get; } = new()
    {
        { EncodeFormat.Mp3, "MP3" },
        { EncodeFormat.Aac, "M4A/AAC" },
        { EncodeFormat.Wav, "WAV" },
        { EncodeFormat.Flac, "FLAC" },
        { EncodeFormat.Ogg, "OGG/Vorbis" },
        { EncodeFormat.Opus, "OPUS" }
    };

    public ListItemCollectionView<int> SampleRateList { get; } = new();

    private void FillSampleRateList(EncodeFormat format)
    {
        var selection = SampleRateList.SelectedValue;
        List<ListItem<int>> list = new() { { 0, "Source" } };
        foreach (var sampleRate in Encoder.GetSupportedSampleRates(format))
        {
            list.Add(sampleRate, string.Format(_environment.CurrentCulture, "{0} Hz", sampleRate));
        }
        
        SampleRateList.Source.Clear();
        SampleRateList.AddRange(list);
        SampleRateList.SelectedValue = SampleRateList.Source.Any(x => x.Value == selection) ? selection : 48000;
    }

    public ListItemCollectionView<int> BitrateList { get; } = new()
    {
        { 0, "Source" },
        { 96, "96 kbps" },
        { 128, "128 kbps" },
        { 192, "192 kbps" },
        { 256, "256 kbps" },
        { 320, "320 kbps" }
    };
    
    public ListItemCollectionView<int> BitsPerSampleList { get; } = new()
    {
        { 8, "8-bits" },
        { 16, "16-bits" },
        { 24, "24-bits" },
        { 32, "32-bits" }
    };

    public ListItemCollectionView<FileExistsAction> FileExistsActionList { get; } = new()
    {
        { FileExistsAction.Ask, "Ask" },
        { FileExistsAction.Skip, "Skip" },
        { FileExistsAction.Overwrite, "Overwrite" },
        { FileExistsAction.Rename, "Rename" },
        { FileExistsAction.Cancel, "Cancel" }
    };
    
    /// <summary>
    /// Enables or disables the Fixed Bitrate option.
    /// </summary>
    public RxCommandUnit ToggleFixedBitrate => _toggleFixedBitrate ??= ReactiveCommand.Create(ToggleFixedBitrateImpl);
    private RxCommandUnit? _toggleFixedBitrate;
    private void ToggleFixedBitrateImpl()
    {
        Settings.Encode.FixedBitrate = !Settings.Encode.FixedBitrate;
    }

    /// <summary>
    /// Starts the batch encoding job.
    /// </summary>
    public RxCommandUnit StartEncoding => _startEncoding ??= ReactiveCommand.CreateFromTask(StartEncodingImplAsync);
    private RxCommandUnit? _startEncoding;
    private Task StartEncodingImplAsync()
    {
        Encoder.ProcessingFiles.Clear();

        // Settings.Encode.Format = FormatsList.SelectedValue;
        // Settings.Encode.Bitrate = BitrateList.SelectedValue;
        // Settings.Encode.BitsPerSample = BitsPerSampleList.SelectedValue;
        // Settings.Encode.SampleRate = SampleRateList.SelectedValue;
        Encoder.FileExistsAction = FileExistsActionList.SelectedValue;
        Encoder.Destination = Settings.Destination;

        return Encoder.RunAsync();
    }

    /// <summary>
    /// Cancels the batch encoding job.
    /// </summary>
    public RxCommandUnit StopEncoding => _stopEncoding ??= ReactiveCommand.Create(StopEncodingImpl);
    private RxCommandUnit? _stopEncoding;
    private void StopEncodingImpl() => Encoder.Cancel();

    /// <inheritdoc />
    protected override Task ShowAboutImplAsync() => _dialogService.ShowAboutAsync(this);

    /// <inheritdoc />
    protected override Task ShowSettingsImplAsync() => _dialogService.ShowSettingsAsync(this, _settings.Value);
    
    /// <inheritdoc />
    public override void OnClosed()
    {
        StopEncoding.Execute().Subscribe();
        base.OnClosed();
    }
}
