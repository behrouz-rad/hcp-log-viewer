// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using System.Text.Json;
using Hcp.LogViewer.App.Models;

namespace Hcp.LogViewer.Tests.TestHelpers;

/// <summary>
/// Helper class for creating test log entries
/// </summary>
internal static class LogEntryFactory
{
    /// <summary>
    /// Creates a simple log entry with basic properties
    /// </summary>
    internal static LogEntry CreateBasicLogEntry(string level = "INFO", string message = "Test message")
    {
        return new LogEntry
        {
            Time = DateTimeOffset.Now,
            Level = level,
            Message = message,
            TraceId = "trace-" + Guid.NewGuid().ToString()[..8],
            SpanId = "span-" + Guid.NewGuid().ToString()[..8]
        };
    }

    /// <summary>
    /// Creates a log entry with attributes
    /// </summary>
    internal static LogEntry CreateLogEntryWithAttributes(
        Dictionary<string, object> attributes,
        string level = "INFO",
        string message = "Test message with attributes")
    {
        var jsonAttributes = new Dictionary<string, JsonElement>();

        foreach (var (key, value) in attributes)
        {
            var jsonValue = JsonSerializer.SerializeToElement(value);
            jsonAttributes.Add(key, jsonValue);
        }

        return new LogEntry
        {
            Time = DateTimeOffset.Now,
            Level = level,
            Message = message,
            TraceId = "trace-" + Guid.NewGuid().ToString()[..8],
            SpanId = "span-" + Guid.NewGuid().ToString()[..8],
            Attributes = jsonAttributes
        };
    }

    /// <summary>
    /// Creates a collection of log entries with different levels
    /// </summary>
    internal static IEnumerable<LogEntry> CreateLogEntryCollection(int count = 5)
    {
        var levels = new[] { "TRACE", "DEBUG", "INFO", "WARN", "ERROR" };
        var result = new List<LogEntry>();

        for (int i = 0; i < count; i++)
        {
            var level = levels[i % levels.Length];
            var message = string.Create(CultureInfo.InvariantCulture, $"Test message {i + 1}");

            var attributes = new Dictionary<string, object>
            {
                ["index"] = i,
                ["isEven"] = i % 2 == 0,
                ["timestamp"] = DateTimeOffset.Now.AddMinutes(-i).ToString("o")
            };

            result.Add(CreateLogEntryWithAttributes(attributes, level, message));
        }

        return result;
    }

    /// <summary>
    /// Creates a JSON string representing a log entry
    /// </summary>
    internal static string CreateLogEntryJson(string level = "INFO", string message = "Test message")
    {
        var entry = CreateBasicLogEntry(level, message);

        return JsonSerializer.Serialize(entry);
    }
}
