// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using System.Text.Json;
using Hcp.LogViewer.App.Models;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.Tests.ViewModels;

public class LogEntryViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var logEntry = new LogEntry
        {
            Time = DateTimeOffset.Parse("2025-01-01T12:00:00Z", CultureInfo.InvariantCulture),
            Level = "INFO",
            Message = "Test message",
            TraceId = "trace123",
            SpanId = "span456",
            Attributes = new Dictionary<string, JsonElement>
            {
                ["userId"] = JsonDocument.Parse("\"user1\"").RootElement,
                ["requestId"] = JsonDocument.Parse("12345").RootElement
            }
        };

        // Act
        var viewModel = new LogEntryViewModel(logEntry, 1);

        // Assert
        viewModel.Time.Should().Be(DateTimeOffset.Parse("2025-01-01T12:00:00Z", CultureInfo.InvariantCulture));
        viewModel.Level.Should().Be("INFO");
        viewModel.Message.Should().Be("Test message");
        viewModel.TraceId.Should().Be("trace123");
        viewModel.SpanId.Should().Be("span456");
        viewModel.Index.Should().Be(1);
        viewModel.FormattedAttributes.Should().Contain("userId");
        viewModel.FormattedAttributes.Should().Contain("user1");
        viewModel.FormattedAttributes.Should().Contain("requestId");
        viewModel.FormattedAttributes.Should().Contain("12345");
    }

    [Fact]
    public void SearchableContent_ShouldCombineAllProperties()
    {
        // Arrange
        var logEntry = new LogEntry
        {
            Time = DateTimeOffset.Parse("2025-01-01T12:00:00Z", CultureInfo.InvariantCulture),
            Level = "INFO",
            Message = "Test message",
            TraceId = "trace123",
            SpanId = "span456",
            Attributes = new Dictionary<string, JsonElement>
            {
                ["userId"] = JsonDocument.Parse("\"user1\"").RootElement
            }
        };

        // Act
        var viewModel = new LogEntryViewModel(logEntry, 1);

        // Assert
        viewModel.SearchableContent.Should().Contain("2025-01-01");
        viewModel.SearchableContent.Should().Contain("INFO");
        viewModel.SearchableContent.Should().Contain("Test message");
        viewModel.SearchableContent.Should().Contain("trace123");
        viewModel.SearchableContent.Should().Contain("span456");
        viewModel.SearchableContent.Should().Contain("userId");
        viewModel.SearchableContent.Should().Contain("user1");
    }

    [Fact]
    public void FormattedAttributes_WithNestedObjects_ShouldFormatCorrectly()
    {
        // Arrange
        const string nestedJson = """
        {
            "user": {
                "id": "user1",
                "roles": ["admin", "user"]
            },
            "request": {
                "id": 12345,
                "params": {
                    "page": 1,
                    "size": 10
                }
            }
        }
        """;

        var logEntry = new LogEntry
        {
            Time = DateTimeOffset.Parse("2025-01-01T12:00:00Z", CultureInfo.InvariantCulture),
            Level = "INFO",
            Attributes = JsonDocument.Parse(nestedJson).RootElement.EnumerateObject()
                .ToDictionary(p => p.Name, p => p.Value)
        };

        // Act
        var viewModel = new LogEntryViewModel(logEntry, 1);

        // Assert
        viewModel.FormattedAttributes.Should().Contain("user");
        viewModel.FormattedAttributes.Should().Contain("id");
        viewModel.FormattedAttributes.Should().Contain("user1");
        viewModel.FormattedAttributes.Should().Contain("roles");
        viewModel.FormattedAttributes.Should().Contain("admin");
        viewModel.FormattedAttributes.Should().Contain("request");
        viewModel.FormattedAttributes.Should().Contain("params");
        viewModel.FormattedAttributes.Should().Contain("page");
        viewModel.FormattedAttributes.Should().Contain("1");
    }

    [Fact]
    public void LogEntryViewModel_WithNullAttributes_ShouldHandleGracefully()
    {
        // Arrange
        var logEntry = new LogEntry
        {
            Time = DateTimeOffset.Parse("2025-01-01T12:00:00Z", CultureInfo.InvariantCulture),
            Level = "INFO",
            Message = "Test message",
            Attributes = null
        };

        // Act
        var viewModel = new LogEntryViewModel(logEntry, 1);

        // Assert
        viewModel.FormattedAttributes.Should().BeEmpty();
        viewModel.SearchableContent.Should().NotContain("null");
    }
}
