// © 2025 Behrouz Rad. All rights reserved.

using Rad.LogViewer.App.Models;

namespace Rad.LogViewer.App.Services.Parsers;

/// <summary>
/// Defines methods for parsing log files.
/// </summary>
/// <remarks>
/// Implementations of this interface should handle reading and parsing log files
/// in an efficient, streaming manner to support large log files.
/// </remarks>
internal interface ILogFileParser
{
    /// <summary>
    /// Streams log entries asynchronously from a file.
    /// </summary>
    /// <param name="filePath">The path to the log file to parse.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An asynchronous stream of log entries.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified log file does not exist.</exception>
    public IAsyncEnumerable<LogEntry> StreamLogEntriesAsync(string filePath, CancellationToken cancellationToken = default);
}
