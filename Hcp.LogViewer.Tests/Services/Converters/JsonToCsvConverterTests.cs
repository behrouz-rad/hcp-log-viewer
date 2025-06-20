// © 2025 Behrouz Rad. All rights reserved.

using Hcp.LogViewer.App.Services.Converters;

namespace Hcp.LogViewer.Tests.Services.Converters;

public class JsonToCsvConverterTests : IDisposable
{
    private readonly string _sampleLogsPath;
    private readonly string _tempCsvPath;

    public JsonToCsvConverterTests()
    {
        _sampleLogsPath = Path.Combine(AppContext.BaseDirectory, "TestData", "sample-logs.json");
        _tempCsvPath = Path.Combine(Path.GetTempPath(), $"test-export-{Guid.NewGuid()}.csv");
    }

    [Fact]
    public async Task ConvertAsync_ShouldCreateValidCsvFile()
    {
        // Arrange
        var converter = new JsonToCsvConverter();

        // Act
        await converter.ConvertAsync(_sampleLogsPath, _tempCsvPath);

        // Assert
        File.Exists(_tempCsvPath).Should().BeTrue();

        var csvContent = await File.ReadAllTextAsync(_tempCsvPath);
        csvContent.Should().NotBeNullOrEmpty();

        // Check for header row
        var lines = csvContent.Split('\n');
        lines.Length.Should().BeGreaterThan(1);

        var headerLine = lines[0];
        headerLine.Should().Contain("time");
        headerLine.Should().Contain("level");
        headerLine.Should().Contain("message");
        headerLine.Should().Contain("traceId");
        headerLine.Should().Contain("attributes.userId");

        // Check for data rows
        var dataLine = lines[1];
        dataLine.Should().Contain("INFO");
        dataLine.Should().Contain("Application started");
    }

    [Fact]
    public async Task ConvertAsync_WithNestedAttributes_ShouldFlattenToColumns()
    {
        // Arrange
        var converter = new JsonToCsvConverter();
        const string jsonContent = """
        {"time":"2025-01-01T12:00:00.000Z","level":"INFO","message":"Test","attributes":{"user":{"id":"user1","roles":["admin"]}}}
        {"time":"2025-01-01T12:01:00.000Z","level":"DEBUG","message":"Test2","attributes":{"user":{"id":"user2","roles":["user"]}}}
        """;

        var jsonPath = Path.Combine(Path.GetTempPath(), $"nested-{Guid.NewGuid()}.json");
        var csvPath = Path.Combine(Path.GetTempPath(), $"nested-{Guid.NewGuid()}.csv");

        await File.WriteAllTextAsync(jsonPath, jsonContent);

        // Act
        await converter.ConvertAsync(jsonPath, csvPath);

        // Assert
        var csvContent = await File.ReadAllTextAsync(csvPath);
        var lines = csvContent.Split('\n');

        var headerLine = lines[0];
        headerLine.Should().Contain("attributes.user.id");
        headerLine.Should().Contain("attributes.user.roles[0]");

        var dataLine = lines[1];
        dataLine.Should().Contain("user1");
        dataLine.Should().Contain("admin");

        // Cleanup
        File.Delete(jsonPath);
        File.Delete(csvPath);
    }

    [Fact]
    public async Task ConvertAsync_WithCancellation_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var converter = new JsonToCsvConverter();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync(); // Cancel immediately

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await converter.ConvertAsync(_sampleLogsPath, _tempCsvPath, cts.Token);
        });
    }

    [Fact]
    public async Task ConvertAsync_WithInvalidJsonPath_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var converter = new JsonToCsvConverter();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await converter.ConvertAsync(nonExistentPath, _tempCsvPath);
        });
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && File.Exists(_tempCsvPath))
        {
            File.Delete(_tempCsvPath);
        }
    }
}
