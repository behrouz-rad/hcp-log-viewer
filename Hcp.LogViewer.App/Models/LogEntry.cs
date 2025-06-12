using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hcp.LogViewer.App.Models;

internal record LogEntry
{
  [JsonPropertyName("time")]
  public DateTimeOffset Time { get; init; }

  [JsonPropertyName("message")]
  public string? Message { get; init; }

  [JsonPropertyName("level")]
  public string? Level { get; init; }

  [JsonPropertyName("traceId")]
  public string? TraceId { get; init; }

  [JsonPropertyName("spanId")]
  public string? SpanId { get; init; }

  [JsonPropertyName("attributes")]
  public Dictionary<string, JsonElement>? Attributes { get; init; }
}
