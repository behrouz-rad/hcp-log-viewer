// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using System.Text.Json;
using Hcp.LogViewer.App.Models;

namespace Hcp.LogViewer.Tests.Models;

public class LogEntryTests
{
    [Fact]
    public void LogEntry_Deserialization_ShouldDeserializeCorrectly()
    {
        // Arrange
        const string json = """
        {
            "time": "2025-01-01T12:00:00.000Z",
            "level": "INFO",
            "message": "Test message",
            "traceId": "trace123",
            "spanId": "span456",
            "attributes": {
                "userId": "user1",
                "requestId": 12345
            }
        }
        """;

        // Act
        var logEntry = JsonSerializer.Deserialize<LogEntry>(json);

        // Assert
        logEntry.Should().NotBeNull();
        logEntry!.Time.Should().Be(DateTimeOffset.Parse("2025-01-01T12:00:00.000Z", CultureInfo.InvariantCulture));
        logEntry.Level.Should().Be("INFO");
        logEntry.Message.Should().Be("Test message");
        logEntry.TraceId.Should().Be("trace123");
        logEntry.SpanId.Should().Be("span456");
        logEntry.Attributes.Should().NotBeNull();
        logEntry.Attributes!.Count.Should().Be(2);
        logEntry.Attributes["userId"].GetString().Should().Be("user1");
        logEntry.Attributes["requestId"].GetInt32().Should().Be(12345);
    }

    [Fact]
    public void LogEntry_WithMinimalData_ShouldDeserializeCorrectly()
    {
        // Arrange
        const string json = """
        {
            "time": "2025-01-01T12:00:00.000Z"
        }
        """;

        // Act
        var logEntry = JsonSerializer.Deserialize<LogEntry>(json);

        // Assert
        logEntry.Should().NotBeNull();
        logEntry!.Time.Should().Be(DateTimeOffset.Parse("2025-01-01T12:00:00.000Z", CultureInfo.InvariantCulture));
        logEntry.Level.Should().BeNull();
        logEntry.Message.Should().BeNull();
        logEntry.TraceId.Should().BeNull();
        logEntry.SpanId.Should().BeNull();
        logEntry.Attributes.Should().BeNull();
    }

    [Fact]
    public void LogEntry_WithNestedAttributes_ShouldDeserializeCorrectly()
    {
        // Arrange
        const string json = """
        {
            "time": "2025-01-01T12:00:00.000Z",
            "level": "INFO",
            "attributes": {
                "user": {
                    "id": "user1",
                    "roles": ["admin", "user"]
                },
                "request": {
                    "id": "req123",
                    "params": {
                        "page": 1,
                        "size": 10
                    }
                }
            }
        }
        """;

        // Act
        var logEntry = JsonSerializer.Deserialize<LogEntry>(json);

        // Assert
        logEntry.Should().NotBeNull();
        logEntry!.Attributes.Should().NotBeNull();
        logEntry.Attributes!.Count.Should().Be(2);
        logEntry.Attributes["user"].ValueKind.Should().Be(JsonValueKind.Object);
        logEntry.Attributes["request"].ValueKind.Should().Be(JsonValueKind.Object);

        var user = logEntry.Attributes["user"];
        user.GetProperty("id").GetString().Should().Be("user1");
        user.GetProperty("roles").ValueKind.Should().Be(JsonValueKind.Array);

        var request = logEntry.Attributes["request"];
        request.GetProperty("id").GetString().Should().Be("req123");
        request.GetProperty("params").GetProperty("page").GetInt32().Should().Be(1);
    }
}
