using Hcp.LogViewer.App.Helpers;
using Hcp.LogViewer.App.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace Hcp.LogViewer.App.ViewModels;

internal sealed class LogEntryViewModel : ViewModelBase
{
  private readonly LogEntry _logEntry;

  public int Index { get; }
  public DateTimeOffset Time => _logEntry.Time;
  public string? Message => _logEntry.Message;
  public string? Level => _logEntry.Level;
  public string? TraceId => _logEntry.TraceId;
  public string? SpanId => _logEntry.SpanId;
  public Dictionary<string, JsonElement>? Attributes => _logEntry.Attributes;

  public string FormattedAttributs => FormatAttributes(Attributes);

  public string? SearchableContent { get; }

  public LogEntryViewModel(LogEntry logEntry, int index)
  {
    _logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
    Index = index;

    SearchableContent = GenerateSearchableContent();
  }

  private string FormatAttributes(Dictionary<string, JsonElement>? attributes)
  {
    if (attributes is null)
    {
      return string.Empty;
    }

    var contentBuilder = new StringBuilder();
    foreach (var attr in attributes)
    {
      contentBuilder.Append($" {attr.Key}={attr.Value.GetRawText()}");
    }

    return contentBuilder.ToString();
  }

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

  private static void AppendIfNotNull(StringBuilder builder, string? value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      builder.Append($" {value}");
    }
  }
}
