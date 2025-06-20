// © 2025 Behrouz Rad. All rights reserved.

using Hcp.LogViewer.App.Models;
using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly Mock<ILogFileParser> _mockLogFileParser;
    private readonly Mock<IFileDialogService> _mockFileDialogService;
    private readonly Mock<IJsonToCsvConverter> _mockJsonToCsvConverter;

    public MainViewModelTests()
    {
        _mockLogFileParser = new Mock<ILogFileParser>();
        _mockFileDialogService = new Mock<IFileDialogService>();
        _mockJsonToCsvConverter = new Mock<IJsonToCsvConverter>();
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        using var viewModel = new MainViewModel(_mockLogFileParser.Object, _mockFileDialogService.Object, _mockJsonToCsvConverter.Object);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.FilteredLogEntries.Should().NotBeNull();
        viewModel.FilteredLogEntries.Should().BeEmpty();
        viewModel.Commands.Should().NotBeNull();
        viewModel.IsLoaded.Should().BeFalse();
        viewModel.IsLoading.Should().BeFalse();
        viewModel.ShowPropertyTitles.Should().BeTrue();
        viewModel.IsCaseSensitive.Should().BeFalse();
    }

    [Fact]
    public async Task LoadLogEntriesAsync_ShouldPopulateEntries()
    {
        // Arrange
        var entries = new List<LogEntry>
        {
            new() { Time = DateTimeOffset.Now, Level = "INFO", Message = "Test message 1" },
            new() { Time = DateTimeOffset.Now.AddMinutes(1), Level = "ERROR", Message = "Test message 2" }
        };

        _mockLogFileParser
            .Setup(p => p.StreamLogEntriesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(entries.ToAsyncEnumerable());

        using var viewModel = new MainViewModel(_mockLogFileParser.Object, _mockFileDialogService.Object, _mockJsonToCsvConverter.Object);

        // Act
        await viewModel.LoadLogEntriesAsync("test.log", CancellationToken.None);

        // Assert - Need to wait for reactive updates
        await Task.Delay(100);

        viewModel.TotalEntryCount.Should().Be(2);
        viewModel.FilteredEntryCount.Should().Be(2);
    }

    [Fact]
    public void CancelPreviousOperation_ShouldCreateNewCancellationToken()
    {
        // Arrange
        using var viewModel = new MainViewModel(_mockLogFileParser.Object, _mockFileDialogService.Object, _mockJsonToCsvConverter.Object);
        var initialToken = viewModel.CancellationToken;

        // Act
        viewModel.CancelPreviousOperation();
        var newToken = viewModel.CancellationToken;

        // Assert
        initialToken.Should().NotBe(newToken);
        initialToken.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void ClearSourceEntries_ShouldResetEntries()
    {
        // Arrange
        var entries = new List<LogEntry>
        {
            new() { Time = DateTimeOffset.Now, Level = "INFO", Message = "Test message" }
        };

        _mockLogFileParser
            .Setup(p => p.StreamLogEntriesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(entries.ToAsyncEnumerable());

        using var viewModel = new MainViewModel(_mockLogFileParser.Object, _mockFileDialogService.Object, _mockJsonToCsvConverter.Object);

        // Act
        viewModel.ClearSourceEntries();

        // Assert
        viewModel.TotalEntryCount.Should().Be(0);
        viewModel.FilteredEntryCount.Should().Be(0);
    }

    [Fact]
    public async Task SearchFiltering_ShouldFilterEntries()
    {
        // Arrange
        var entries = new List<LogEntry>
        {
            new() { Time = DateTimeOffset.Now, Level = "INFO", Message = "First test message" },
            new() { Time = DateTimeOffset.Now.AddMinutes(1), Level = "ERROR", Message = "Second error message" },
            new() { Time = DateTimeOffset.Now.AddMinutes(2), Level = "WARN", Message = "Third warning message" }
        };

        _mockLogFileParser
            .Setup(p => p.StreamLogEntriesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(entries.ToAsyncEnumerable());

        using var viewModel = new MainViewModel(_mockLogFileParser.Object, _mockFileDialogService.Object, _mockJsonToCsvConverter.Object);

        // Act - Load entries first
        await viewModel.LoadLogEntriesAsync("test.log", CancellationToken.None);

        // Wait for reactive updates
        await Task.Delay(100);

        // Initial state
        viewModel.FilteredEntryCount.Should().Be(3);

        // Act - Apply search filter
        viewModel.SearchAllText = "error";

        // Wait for throttled search to complete
        await Task.Delay(500);

        // Assert
        viewModel.FilteredEntryCount.Should().Be(1);

        // Act - Clear search
        viewModel.SearchAllText = "";

        // Wait for throttled search to complete
        await Task.Delay(500);

        // Assert
        viewModel.FilteredEntryCount.Should().Be(3);
    }

    [Fact]
    public void Dispose_ShouldCleanupResources()
    {
        // Arrange
        var viewModel = new MainViewModel(_mockLogFileParser.Object, _mockFileDialogService.Object, _mockJsonToCsvConverter.Object);

        // Act
        viewModel.Dispose();

        // Assert - No exception should be thrown
        true.Should().BeTrue();
    }
}
