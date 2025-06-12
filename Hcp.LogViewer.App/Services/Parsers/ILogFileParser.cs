using Hcp.LogViewer.App.Models;

namespace Hcp.LogViewer.App.Services.Parsers;

internal interface ILogFileParser
{
    public IAsyncEnumerable<LogEntry> StreamLogEntriesAsync(string filePath, CancellationToken cancellationToken = default);
}
