// © 2025 Behrouz Rad. All rights reserved.

using Rad.LogViewer.App.Services.Converters;
using Rad.LogViewer.App.Services.Dialogs;
using Rad.LogViewer.App.Services.Parsers;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.Services.Theme;
using Rad.LogViewer.App.ViewModels;
using Rad.LogViewer.Tests.TestHelpers;

namespace Rad.LogViewer.Tests.Integration;

public class LogViewerIntegrationTests : IDisposable
{
    private readonly string _testLogFilePath;
    private readonly string _testCsvFilePath;

    public LogViewerIntegrationTests()
    {
        // Create temporary files for testing
        _testLogFilePath = Path.Combine(Path.GetTempPath(), $"test-log-{Guid.NewGuid()}.json");
        _testCsvFilePath = Path.Combine(Path.GetTempPath(), $"test-export-{Guid.NewGuid()}.csv");

        // Create test log file with sample entries
        var logEntries = new[]
        {
            LogEntryFactory.CreateLogEntryJson("INFO", "Application started"),
            LogEntryFactory.CreateLogEntryJson("DEBUG", "Processing request"),
            LogEntryFactory.CreateLogEntryJson("WARN", "Slow operation detected"),
            LogEntryFactory.CreateLogEntryJson("ERROR", "Exception occurred"),
            LogEntryFactory.CreateLogEntryJson("INFO", "Operation completed")
        };

        File.WriteAllLines(_testLogFilePath, logEntries);
    }

    [Fact]
    public async Task EndToEnd_ParseAndConvertLogFile()
    {
        // Arrange
        var parser = new LogFileParser();
        var converter = new JsonToCsvConverter();

        // Act - Parse the log file
        var entries = await parser.StreamLogEntriesAsync(_testLogFilePath).ToListAsync();

        // Assert - Verify parsing results
        entries.Should().NotBeNull();
        entries.Count.Should().Be(5);
        entries.Count(e => string.Equals(e.Level, "INFO", StringComparison.OrdinalIgnoreCase)).Should().Be(2);
        entries.Count(e => string.Equals(e.Level, "ERROR", StringComparison.OrdinalIgnoreCase)).Should().Be(1);

        // Act - Convert to CSV
        await converter.ConvertAsync(_testLogFilePath, _testCsvFilePath);

        // Assert - Verify CSV file was created
        File.Exists(_testCsvFilePath).Should().BeTrue();

        var csvContent = await File.ReadAllTextAsync(_testCsvFilePath);
        csvContent.Should().NotBeNullOrEmpty();

        // CSV should have a header row and 5 data rows
        var lines = csvContent.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        lines.Length.Should().Be(6); // Header + 5 entries

        // Header should contain expected columns
        lines[0].Should().Contain("time");
        lines[0].Should().Contain("level");
        lines[0].Should().Contain("message");

        // Data rows should contain expected values
        lines.Any(l => l.Contains("Application started", StringComparison.InvariantCultureIgnoreCase)).Should().BeTrue();
        lines.Any(l => l.Contains("ERROR", StringComparison.InvariantCultureIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task Integration_ParserWithViewModel()
    {
        // Arrange
        var parser = new LogFileParser();
        var mockFileDialogService = new Mock<IFileDialogService>();
        var mockJsonToCsvConverter = new Mock<IJsonToCsvConverter>();
        var themeService = new Mock<IThemeService>();
        var settingsService = new Mock<ISettingsService>();

        using var viewModel = new MainViewModel(parser,
                                                mockFileDialogService.Object,
                                                mockJsonToCsvConverter.Object,
                                                themeService.Object,
                                                settingsService.Object);

        // Act
        await viewModel.LoadLogEntriesAsync(_testLogFilePath, CancellationToken.None);

        // Wait for reactive updates
        await Task.Delay(100);

        // Assert
        viewModel.TotalEntryCount.Should().Be(5);
        viewModel.FilteredEntryCount.Should().Be(5);

        // Test filtering
        viewModel.SearchAllText = "ERROR";

        // Wait for throttled search to complete
        await Task.Delay(500);

        // Assert filtered results
        viewModel.FilteredEntryCount.Should().Be(1);

        // Test field search
        viewModel.SearchAllText = string.Empty;
        viewModel.IsFieldSearch = true;
        viewModel.LogLevel = App.Models.LogLevel.WARN;

        // Wait for throttled search to complete
        await Task.Delay(500);

        // Assert filtered results
        viewModel.FilteredEntryCount.Should().Be(1);
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
            if (File.Exists(_testLogFilePath))
            {
                File.Delete(_testLogFilePath);
            }

            if (File.Exists(_testCsvFilePath))
            {
                File.Delete(_testCsvFilePath);
            }
        }
    }
}
