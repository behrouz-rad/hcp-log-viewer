using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DynamicData;
using DynamicData.Binding;
using Hcp.LogViewer.App.Models;
using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace Hcp.LogViewer.App.ViewModels;

internal class MainViewModel : ViewModelBase, IDisposable
{
    private readonly ILogFileParser _logFileParser;
    private readonly IFileDialogService _fileDialogService;
    private readonly IJsonToCsvConverter _jsonToCsvConverter;

    private readonly ObservableAsPropertyHelper<int>? _totalEntryCount;
    private readonly SourceList<(LogEntry, int Index)> _sourceLogEntries = new();
    private readonly ReadOnlyObservableCollection<LogEntryViewModel>? _filteredLogEntries;
    public LogLevel[] LogLevels { get; } = Enum.GetValues<LogLevel>();

    private CancellationTokenSource? _cts;

    private string _selectedFilePath = "No file selected.";
    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set => this.RaiseAndSetIfChanged(ref _selectedFilePath, value);
    }

    private string? _searchAllText;
    public string? SearchAllText
    {
        get => _searchAllText;
        set => this.RaiseAndSetIfChanged(ref _searchAllText, value);
    }

    private string? _effectiveSearchTerm;
    public string? EffectiveSearchTerm
    {
        get => _effectiveSearchTerm;
        private set => this.RaiseAndSetIfChanged(ref _effectiveSearchTerm, value);
    }

    private string? _messageSearchText;
    public string? MessageSearchText
    {
        get => _messageSearchText;
        set => this.RaiseAndSetIfChanged(ref _messageSearchText, value);
    }

    private LogLevel? _logLevel;
    public LogLevel? LogLevel
    {
        get => _logLevel;
        set => this.RaiseAndSetIfChanged(ref _logLevel, value);
    }

    private string? _attributesSearchText;
    public string? AttributesSearchText
    {
        get => _attributesSearchText;
        set => this.RaiseAndSetIfChanged(ref _attributesSearchText, value);
    }

    private DateTimeOffset? _dateSearch;
    public DateTimeOffset? DateSearch
    {
        get => _dateSearch;
        set => this.RaiseAndSetIfChanged(ref _dateSearch, value);
    }

    private bool _isFieldSearch;
    public bool IsFieldSearch
    {
        get => _isFieldSearch;
        set => this.RaiseAndSetIfChanged(ref _isFieldSearch, value);
    }

    public int TotalEntryCount => _totalEntryCount?.Value ?? 0;

    public int FilteredEntryCount => _filteredLogEntries?.Count ?? 0;

    private bool _isLoading;
    public bool IsLoading
    {
        get { return _isLoading; }
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private bool _isLoaded;
    public bool IsLoaded
    {
        get { return _isLoaded; }
        set => this.RaiseAndSetIfChanged(ref _isLoaded, value);
    }

    public ReadOnlyObservableCollection<LogEntryViewModel>? FilteredLogEntries => _filteredLogEntries;

    public ReactiveCommand<Window, Unit> OpenFileCommand { get; } = null!;
    public ReactiveCommand<(string filePath, Window owner), Unit> OpenFileWithPathCommand { get; } = null!;
    public ReactiveCommand<Window, Unit> ExportToCsvCommand { get; } = null!;
    public ReactiveCommand<Unit, Unit> ClearSearchCommand { get; } = null!;
    public ReactiveCommand<Unit, Unit> ExitCommand { get; } = ReactiveCommand.Create(ExitApp);
    public ReactiveCommand<Window, Unit> ShowAboutCommand { get; } = ReactiveCommand.CreateFromTask<Window>(ShowAboutAsync);

    public MainViewModel() // For design-time  
    {
        if (Design.IsDesignMode)
        {
            _sourceLogEntries.AddRange(
            [
                   (new LogEntry { Time = DateTimeOffset.Now, Level = "INFO", Message = "Design Time Message 1" }, 0),
                   (new LogEntry { Time = DateTimeOffset.Now.AddSeconds(-5), Level = "WARN", Message = "Design Time Message 2" }, 1)
            ]);
        }
    }

    public MainViewModel(ILogFileParser jsonFileService, IFileDialogService fileDialogService, IJsonToCsvConverter jsonToCsvConverter) // For runtime
    {
        _logFileParser = jsonFileService ?? throw new ArgumentNullException(nameof(jsonFileService));
        _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
        _jsonToCsvConverter = jsonToCsvConverter ?? throw new ArgumentNullException(nameof(jsonToCsvConverter));
        _cts = new CancellationTokenSource();

        this.WhenAnyValue(x => x.SearchAllText,
                          x => x.MessageSearchText,
                          x => x.IsFieldSearch)
            .Subscribe(_ => UpdateEffectiveSearchTerm());

        var filterPredicate = CreateFilterPredicate();

        _sourceLogEntries.Connect()
            .Transform(logEntry => new LogEntryViewModel(logEntry.Item1, logEntry.Index))
            .Filter(filterPredicate)
            .Sort(SortExpressionComparer<LogEntryViewModel>.Ascending(logEntry => logEntry.Index))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _filteredLogEntries)
            .DisposeMany()
            .Subscribe();

        _filteredLogEntries
            .ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(FilteredEntryCount)));

        _sourceLogEntries
            .CountChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.TotalEntryCount, out _totalEntryCount);

        OpenFileCommand = ReactiveCommand.CreateFromTask<Window>(OpenFileAsync);
        OpenFileWithPathCommand = ReactiveCommand.CreateFromTask<(string, Window)>(OpenFileWithPathAsync);
        ExportToCsvCommand = ReactiveCommand.CreateFromTask<Window>(ExportToCsvAsync);
        ClearSearchCommand = ReactiveCommand.Create(ClearSearch);
    }

    private static Func<LogEntryViewModel, bool> FilterAll(string? term)
    {
        if (string.IsNullOrWhiteSpace(term)) return _ => true;

        return item => item.SearchableContent?.Contains(term, StringComparison.OrdinalIgnoreCase) == true;
    }

    private void UpdateEffectiveSearchTerm()
    {
        var searchText = IsFieldSearch ? MessageSearchText : SearchAllText;
        EffectiveSearchTerm = string.IsNullOrWhiteSpace(searchText) ? null : searchText;
    }

    private static Func<LogEntryViewModel, bool> FilterByCriteria(string? message, string? level, string? attributes, DateTimeOffset? date)
    {
        return item =>
        {
            bool matchesMessage = string.IsNullOrWhiteSpace(message) || item.Message?.Contains(message, StringComparison.OrdinalIgnoreCase) == true;
            bool matchesLevel = string.IsNullOrWhiteSpace(level) || item.Level?.Equals(level, StringComparison.OrdinalIgnoreCase) == true;
            bool matchesAttributes = string.IsNullOrWhiteSpace(attributes) || item.FormattedAttributs.Contains(attributes, StringComparison.OrdinalIgnoreCase);
            bool matchesDate = !date.HasValue || item.Time.Date == date.Value.Date;

            return matchesMessage && matchesLevel && matchesAttributes && matchesDate;
        };
    }

    private IObservable<Func<LogEntryViewModel, bool>> CreateFilterPredicate()
    {
        return this.WhenAnyValue(
                x => x.SearchAllText,
                x => x.MessageSearchText,
                x => x.LogLevel,
                x => x.AttributesSearchText,
                x => x.DateSearch,
                x => x.IsFieldSearch)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Select(values =>
            {
                return IsFieldSearch
                ? FilterByCriteria(MessageSearchText, LogLevel.ToString(), AttributesSearchText, DateSearch)
                : FilterAll(SearchAllText);
            });
    }

    private void CancelPreviousOperation()
    {
        if (_cts is not null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        _cts = new CancellationTokenSource();
    }

    private Task LoadLogEntriesAsync(string filePath, CancellationToken cancellationToken)
    {
        int index = 0;
        return Task.Run(async () =>
        {
            await foreach (var item in _logFileParser.StreamLogEntriesAsync(filePath, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested) break;
                _sourceLogEntries.Add((item, index++));
            }
        }, cancellationToken);
    }

    public async Task OpenFileAsync(Window window)
    {
        CancelPreviousOperation();

        var filePathResult = await _fileDialogService.ShowOpenFileDialogAsync();
        if (filePathResult.IsFailed)
        {
            return;
        }

        var filePath = filePathResult.Value;

        await OpenFileWithPathAsync((filePath, window));
    }

    public async Task OpenFileWithPathAsync((string filePath, Window window) param)
    {
        IsLoading = true;

        CancelPreviousOperation();

        _sourceLogEntries.Clear();

        try
        {
            await LoadLogEntriesAsync(param.filePath, _cts!.Token);

            SelectedFilePath = param.filePath;
            IsLoaded = true;
        }
        catch (OperationCanceledException)
        {
            SelectedFilePath = $"Loading cancelled for {param.filePath}";

            var msgBox = MessageBoxManager
                        .GetMessageBoxStandard("HCP Log Viewer", $"Loading cancelled for {param.filePath}",
            ButtonEnum.Ok, Icon.Warning);

            await msgBox.ShowWindowDialogAsync(param.window);
        }
        catch (Exception ex)
        {
            var msgBox = MessageBoxManager
                        .GetMessageBoxStandard("HCP Log Viewer", $"Error loading file: {ex.Message}",
                                ButtonEnum.Ok, Icon.Error);

            await msgBox.ShowWindowDialogAsync(param.window);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void ClearSearch()
    {
        SearchAllText = null;
        MessageSearchText = null;
        LogLevel = null;
        AttributesSearchText = null;
        DateSearch = null;
    }

    public static Task ShowAboutAsync(Window owner)
    {
        var aboutWindow = new Views.AboutWindow();
        return aboutWindow.ShowDialog(owner);
    }

    public static void ExitApp()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }

    public async Task ExportToCsvAsync(Window window)
    {
        if (!IsLoaded)
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Export to CSV",
                "Please open a log file first.",
                ButtonEnum.Ok,
                Icon.Warning);

            await msgBox.ShowWindowDialogAsync(window);
            return;
        }

        var filePathResult = await _fileDialogService.ShowSaveFileDialogAsync(".csv",
            Path.GetFileNameWithoutExtension(SelectedFilePath) + ".csv");

        if (filePathResult.IsFailed)
        {
            return;
        }

        var csvFilePath = filePathResult.Value;

        try
        {
            IsLoading = true;
            await _jsonToCsvConverter.ConvertAsync(SelectedFilePath, csvFilePath);
            IsLoading = false;

            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Export to CSV",
                $"Successfully exported to {csvFilePath}",
                ButtonEnum.Ok,
                Icon.Info);

            await msgBox.ShowWindowDialogAsync(window);

        }
        catch (Exception ex)
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Export to CSV",
                $"Error exporting to CSV: {ex.Message}",
                ButtonEnum.Ok,
                Icon.Error);

            await msgBox.ShowWindowDialogAsync(window);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _sourceLogEntries.Dispose();
        }
    }
}
