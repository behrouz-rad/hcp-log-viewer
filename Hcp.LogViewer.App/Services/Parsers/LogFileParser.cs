// © 2025 Behrouz Rad. All rights reserved.

using System.Runtime.CompilerServices;
using System.Text.Json;
using Hcp.LogViewer.App.Models;

namespace Hcp.LogViewer.App.Services.Parsers;

/// <summary>
/// Implements the log file parser for JSON-formatted log files.
/// </summary>
/// <remarks>
/// This parser reads log files line by line, parsing each line as a JSON object
/// that can be deserialized into a <see cref="LogEntry"/>.
/// </remarks>
internal sealed class LogFileParser : ILogFileParser
{
    /// <summary>
    /// Streams log entries asynchronously from a file.
    /// </summary>
    /// <param name="filePath">The path to the log file to parse.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous stream of log entries.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified log file does not exist.</exception>
    /// <remarks>
    /// This method reads the file line by line and parses each line as a JSON object.
    /// Empty lines are skipped. The method uses streaming to efficiently handle large files.
    /// </remarks>
    /// <example>
    /// <code>
    /// await foreach (var entry in parser.StreamLogEntriesAsync("app.log"))
    /// {
    ///     Console.WriteLine($"{entry.Time}: {entry.Message}");
    /// }
    /// </code>
    /// </example>
    public async IAsyncEnumerable<LogEntry> StreamLogEntriesAsync(
                string filePath,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Log file not found.", filePath);
        }

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                              bufferSize: 4096, useAsync: true);
        using var reader = new StreamReader(fileStream);

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            cancellationToken.ThrowIfCancellationRequested();

            if (JsonSerializer.Deserialize<LogEntry>(line, options) is LogEntry entry)
            {
                yield return entry;
            }
        }
    }
}
