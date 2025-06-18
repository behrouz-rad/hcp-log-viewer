// © 2025 Behrouz Rad. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hcp.LogViewer.App.Models;

/// <summary>
/// Represents a structured log entry from a log file.
/// </summary>
internal sealed record LogEntry
{
    /// <summary>
    /// Gets the timestamp when the log entry was created.
    /// </summary>
    /// <value>A DateTimeOffset representing the log entry timestamp.</value>
    [JsonPropertyName("time")]
    public required DateTimeOffset Time { get; init; }

    /// <summary>
    /// Gets the log message content.
    /// </summary>
    /// <value>The text message of the log entry, or null if no message was provided.</value>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// Gets the severity level of the log entry.
    /// </summary>
    /// <value>A string representation of the log level (e.g., "INFO", "ERROR").</value>
    [JsonPropertyName("level")]
    public string? Level { get; init; }

    /// <summary>
    /// Gets the trace identifier for distributed tracing.
    /// </summary>
    /// <value>A unique identifier for correlating related log entries across services.</value>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }

    /// <summary>
    /// Gets the span identifier for distributed tracing.
    /// </summary>
    /// <value>A unique identifier for a specific operation within a trace.</value>
    [JsonPropertyName("spanId")]
    public string? SpanId { get; init; }

    /// <summary>
    /// Gets the collection of additional attributes associated with this log entry.
    /// </summary>
    /// <value>
    /// A dictionary of key-value pairs representing structured data attached to the log entry.
    /// May be null if no attributes were included.
    /// </value>
    [JsonPropertyName("attributes")]
    public Dictionary<string, JsonElement>? Attributes { get; init; }
}
