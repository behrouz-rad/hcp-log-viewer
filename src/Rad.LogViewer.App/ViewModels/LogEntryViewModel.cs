// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using System.Text;
using System.Text.Json;
using Rad.LogViewer.App.Helpers;
using Rad.LogViewer.App.Models;

namespace Rad.LogViewer.App.ViewModels;

/// <summary>
/// View model for a log entry in the log viewer.
/// </summary>
internal sealed class LogEntryViewModel : ViewModelBase
{
    private readonly LogEntry _logEntry;

    /// <summary>
    /// Gets the index of the log entry in the collection.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the timestamp of the log entry.
    /// </summary>
    public DateTimeOffset Time => _logEntry.Time;

    /// <summary>
    /// Gets the message content of the log entry.
    /// </summary>
    public string? Message => _logEntry.Message;

    /// <summary>
    /// Gets the log level of the entry.
    /// </summary>
    public string? Level => _logEntry.Level;

    /// <summary>
    /// Gets the trace identifier for distributed tracing.
    /// </summary>
    public string? TraceId => _logEntry.TraceId;

    /// <summary>
    /// Gets the span identifier for distributed tracing.
    /// </summary>
    public string? SpanId => _logEntry.SpanId;

    /// <summary>
    /// Gets the additional attributes associated with the log entry.
    /// </summary>
    public Dictionary<string, JsonElement>? Attributes => _logEntry.Attributes;

    /// <summary>
    /// Gets a formatted string representation of the log attributes.
    /// </summary>
    public string FormattedAttributes => FormatAttributes(Attributes);

    /// <summary>
    /// Gets a normalized string containing all log entry data for searching.
    /// </summary>
    public string? SearchableContent { get; }

    /// <summary>
    /// Initializes a new instance of the LogEntryViewModel class.
    /// </summary>
    /// <param name="logEntry">The log entry model.</param>
    /// <param name="index">The index of the entry in the collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when logEntry is null.</exception>
    public LogEntryViewModel(LogEntry logEntry, int index)
    {
        _logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
        Index = index;

        SearchableContent = GenerateSearchableContent();
    }

    /// <summary>
    /// Formats the attributes dictionary into a string representation.
    /// </summary>
    /// <param name="attributes">The attributes to format.</param>
    /// <returns>A string representation of the attributes.</returns>
    private static string FormatAttributes(Dictionary<string, JsonElement>? attributes)
    {
        if (attributes is null)
        {
            return string.Empty;
        }

        var contentBuilder = new StringBuilder();
        foreach (var attr in attributes)
        {
            contentBuilder.Append(CultureInfo.InvariantCulture, $" {attr.Key}={attr.Value.GetRawText()}");
        }

        return contentBuilder.ToString();
    }

    /// <summary>
    /// Generates a searchable string containing all log entry data.
    /// </summary>
    /// <returns>A normalized string for searching.</returns>
    private string GenerateSearchableContent()
    {
        var contentBuilder = new StringBuilder();

        // Append time in ISO 8601 format
        contentBuilder.Append(Time.ToString("o"));

        // Append non-null properties
        AppendIfNotNull(contentBuilder, Message);
        AppendIfNotNull(contentBuilder, Level);
        AppendIfNotNull(contentBuilder, TraceId);
        AppendIfNotNull(contentBuilder, SpanId);

        // Append attributes if available
        contentBuilder.Append(FormatAttributes(Attributes));

        // Normalize whitespace
        return WhitespaceRegEx.EveryWhitespace()
                              .Replace(contentBuilder.ToString(), " ")
                              .Trim();
    }

    /// <summary>
    /// Appends a string to the StringBuilder if it's not null or empty.
    /// </summary>
    /// <param name="builder">The StringBuilder to append to.</param>
    /// <param name="value">The value to append.</param>
    private static void AppendIfNotNull(StringBuilder builder, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            builder.Append(CultureInfo.InvariantCulture, $" {value}");
        }
    }
}
