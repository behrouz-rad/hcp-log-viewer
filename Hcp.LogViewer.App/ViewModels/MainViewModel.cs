// © 2025 Behrouz Rad. All rights reserved.

using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using Hcp.LogViewer.App.Commands;
using Hcp.LogViewer.App.Models;
using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.Services.Settings;
using Hcp.LogViewer.App.Services.Theme;
using ReactiveUI;

namespace Hcp.LogViewer.App.ViewModels;

internal class MainViewModel : ViewModelBase, IDisposable
{
    private readonly ILogFileParser _logFileParser;
    private readonly IThemeService _themeService;

    private readonly ObservableAsPropertyHelper<int>? _totalEntryCount;
    private readonly SourceList<(LogEntry, int Index)> _sourceLogEntries = new();
    private readonly ReadOnlyObservableCollection<LogEntryViewModel>? _filteredLogEntries;
    public LogLevel[] LogLevels { get; } = Enum.GetValues<LogLevel>();

    private CancellationTokenSource? _cancellationTokenSource;
    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

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

    private bool _showPropertyTitles = true;
    public bool ShowPropertyTitles
    {
        get { return _showPropertyTitles; }
        set => this.RaiseAndSetIfChanged(ref _showPropertyTitles, value);
    }

    private bool _isCaseSensitive;
    public bool IsCaseSensitive
    {
        get { return _isCaseSensitive; }
        set => this.RaiseAndSetIfChanged(ref _isCaseSensitive, value);
    }

    public bool IsDarkTheme => _themeService?.IsDarkTheme ?? false;

    public ReadOnlyObservableCollection<LogEntryViewModel>? FilteredLogEntries => _filteredLogEntries;

    public AppCommands Commands { get; }

    public ReactiveCommand<Window, Unit> OpenFileCommand => Commands.OpenFile.Command;
    public ReactiveCommand<(string filePath, Window owner), Unit> OpenFileWithPathCommand => Commands.OpenFileWithPath.Command;
    public ReactiveCommand<Window, Unit> ExportToCsvCommand => Commands.ExportToCsv.Command;
    public ReactiveCommand<Unit, Unit> ClearSearchCommand => Commands.ClearSearch.Command;
    public ReactiveCommand<Unit, Unit> ExitCommand => Commands.Exit.Command;
    public ReactiveCommand<Window, Unit> ShowAboutCommand => Commands.ShowAbout.Command;
    public ReactiveCommand<LogEntryViewModel, Unit> CopyLogEntryCommand => Commands.CopyLogEntry.Command;
    public ReactiveCommand<Unit, Unit> ToggleThemeCommand => Commands.ToggleTheme.Command;
    public ReactiveCommand<Unit, Unit> TogglePropertyTitlesCommand => Commands.TogglePropertyTitles.Command;

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

    public MainViewModel(ILogFileParser logFileParser, IFileDialogService fileDialogService,
                        IJsonToCsvConverter jsonToCsvConverter, IThemeService themeService,
                        ISettingsService settingsService) // For runtime
    {
        _logFileParser = logFileParser;
        _themeService = themeService;

        _cancellationTokenSource = new CancellationTokenSource();

        _showPropertyTitles = settingsService.GetSetting(Constants.AppConstants.Settings.ShowPropertyTitles, true);

        Commands = CommandFactory.CreateCommands(this, fileDialogService, jsonToCsvConverter, themeService, settingsService);

        this.WhenAnyValue(x => x.SearchAllText,
                          x => x.MessageSearchText,
                          x => x.IsFieldSearch,
                          x => x.IsCaseSensitive)
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
    }

    private static Func<LogEntryViewModel, bool> FilterAll(string? term, bool isCaseSensitive)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return _ => true;
        }

        var comparison = isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return item => item.SearchableContent?.Contains(term, comparison) == true;
    }

    private void UpdateEffectiveSearchTerm()
    {
        var searchText = IsFieldSearch ? MessageSearchText : SearchAllText;
        EffectiveSearchTerm = string.IsNullOrWhiteSpace(searchText) ? null : searchText;
    }

    private static Func<LogEntryViewModel, bool> FilterByCriteria(string? message, string? level, string? attributes, DateTimeOffset? date, bool isCaseSensitive)
    {
        return item =>
        {
            var comparison = isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            bool matchesMessage = string.IsNullOrWhiteSpace(message) || item.Message?.Contains(message, comparison) == true;
            bool matchesLevel = string.IsNullOrWhiteSpace(level) || item.Level?.Equals(level, StringComparison.OrdinalIgnoreCase) == true;
            bool matchesAttributes = string.IsNullOrWhiteSpace(attributes) || item.FormattedAttributes.Contains(attributes, comparison);
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
                x => x.IsFieldSearch,
                x => x.IsCaseSensitive)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Select(_ =>
            {
                return IsFieldSearch
                ? FilterByCriteria(MessageSearchText, LogLevel.ToString(), AttributesSearchText, DateSearch, IsCaseSensitive)
                : FilterAll(SearchAllText, IsCaseSensitive);
            });
    }

    public virtual void CancelPreviousOperation()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task LoadLogEntriesAsync(string filePath, CancellationToken cancellationToken)
    {
        int index = 0;
        return Task.Run(async () =>
        {
            await foreach (var item in _logFileParser.StreamLogEntriesAsync(filePath, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _sourceLogEntries.Add((item, index++));
            }
        }, cancellationToken);
    }

    public void ClearSourceEntries()
    {
        _sourceLogEntries.Clear();
    }

    public virtual async Task OpenFileWithPathAsync((string filePath, Window window) param)
    {
        await Commands.OpenFileWithPath.Command.Execute(param);
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
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _sourceLogEntries?.Dispose();
        }
    }
}
