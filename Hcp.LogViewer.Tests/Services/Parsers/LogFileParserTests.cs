// © 2025 Behrouz Rad. All rights reserved.

using System.Text.Json;
using Hcp.LogViewer.App.Services.Parsers;

namespace Hcp.LogViewer.Tests.Services.Parsers;

public class LogFileParserTests
{
    private readonly string _sampleLogsPath;
    private readonly string _invalidLogsPath;

    public LogFileParserTests()
    {
        _sampleLogsPath = Path.Combine(AppContext.BaseDirectory, "TestData", "sample-logs.json");
        _invalidLogsPath = Path.Combine(AppContext.BaseDirectory, "TestData", "invalid-logs.json");
    }

    [Fact]
    public async Task StreamLogEntriesAsync_WithValidFile_ShouldReturnAllEntries()
    {
        // Arrange
        var parser = new LogFileParser();

        // Act
        var entries = await parser.StreamLogEntriesAsync(_sampleLogsPath).ToListAsync();

        // Assert
        entries.Should().NotBeNull();
        entries.Count.Should().Be(5);

        // Verify first entry
        var firstEntry = entries.First();
        firstEntry.Level.Should().Be("INFO");
        firstEntry.Message.Should().Be("Application started");
        firstEntry.TraceId.Should().Be("trace123");
        firstEntry.SpanId.Should().Be("span456");
        firstEntry.Attributes.Should().NotBeNull();
        firstEntry.Attributes!["userId"].GetString().Should().Be("user1");

        // Verify last entry
        var lastEntry = entries.Last();
        lastEntry.Level.Should().Be("INFO");
        lastEntry.Message.Should().Be("Request completed");
        lastEntry.Attributes!["statusCode"].GetInt32().Should().Be(200);
    }

    [Fact]
    public async Task StreamLogEntriesAsync_WithInvalidFile_ShouldSkipInvalidLines()
    {
        // Arrange
        var parser = new LogFileParser();

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(async () =>
        {
            await parser.StreamLogEntriesAsync(_invalidLogsPath).ToListAsync();
        });
    }

    [Fact]
    public async Task StreamLogEntriesAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var parser = new LogFileParser();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await parser.StreamLogEntriesAsync(nonExistentPath).ToListAsync();
        });
    }

    [Fact]
    public async Task StreamLogEntriesAsync_WithCancellation_ShouldStopProcessing()
    {
        // Arrange
        var parser = new LogFileParser();
        using var cts = new CancellationTokenSource();

        // Act
        var enumerator = parser.StreamLogEntriesAsync(_sampleLogsPath, cts.Token).GetAsyncEnumerator();

        // Get first item
        var hasFirst = await enumerator.MoveNextAsync();
        hasFirst.Should().BeTrue();

        // Cancel before getting second item
        await cts.CancelAsync();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await enumerator.MoveNextAsync();
        });
    }
}
