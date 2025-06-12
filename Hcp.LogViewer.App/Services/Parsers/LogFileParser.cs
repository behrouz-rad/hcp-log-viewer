using System.Runtime.CompilerServices;
using System.Text.Json;
using Hcp.LogViewer.App.Models;

namespace Hcp.LogViewer.App.Services.Parsers;

internal sealed class LogFileParser : ILogFileParser
{
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

            LogEntry? entry = JsonSerializer.Deserialize<LogEntry>(line, options);
            if (entry != null)
            {
                yield return entry;
            }
        }
    }
}
