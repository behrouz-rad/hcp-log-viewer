using System.Text;
using System.Text.Json;

namespace Hcp.LogViewer.App.Services.Converters;

public class JsonToCsvConverter : IJsonToCsvConverter
{
    public async Task ConvertAsync(string jsonFilePath, string csvFilePath, CancellationToken cancellationToken = default)
    {
        var headers = await CollectHeadersAsync(jsonFilePath, cancellationToken);

        await WriteCsvAsync(jsonFilePath, csvFilePath, headers, cancellationToken);
    }

    private async Task<HashSet<string>> CollectHeadersAsync(string jsonFilePath, CancellationToken cancellationToken)
    {
        var headers = new HashSet<string>();
        using var reader = new StreamReader(jsonFilePath);
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var doc = JsonDocument.Parse(line);
            var record = new Dictionary<string, string>();
            FlattenJsonElement(doc.RootElement, record);
            foreach (var key in record.Keys)
                headers.Add(key);
        }

        return headers;
    }

    private async Task WriteCsvAsync(string jsonFilePath, string csvFilePath, HashSet<string> headers, CancellationToken cancellationToken)
    {
        var headerList = headers.ToList();
        using var reader = new StreamReader(jsonFilePath);
        using var writer = new StreamWriter(csvFilePath);

        // Write header
        await writer.WriteLineAsync(string.Join(",", headerList.Select(EscapeCsv)).AsMemory(), cancellationToken);

        // Write records
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var doc = JsonDocument.Parse(line);
            var record = new Dictionary<string, string>();
            FlattenJsonElement(doc.RootElement, record);

            var csvLine = new StringBuilder();
            for (int i = 0; i < headerList.Count; i++)
            {
                if (i > 0) csvLine.Append(',');
                record.TryGetValue(headerList[i], out var value);
                csvLine.Append(EscapeCsv(value));
            }
            await writer.WriteLineAsync(csvLine, cancellationToken);
        }
    }

    private static string EscapeCsv(string? value)
    {
        if (value == null)
            return "\"\"";
        // Escape double quotes by doubling them
        var escaped = value.Replace("\"", "\"\"");
        return $"\"{escaped}\"";
    }

    private void FlattenJsonElement(JsonElement element, Dictionary<string, string> dict, string parentKey = "")
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    string key = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}.{property.Name}";
                    FlattenJsonElement(property.Value, dict, key);
                }
                break;
            case JsonValueKind.Array:
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    string key = $"{parentKey}[{index}]";
                    FlattenJsonElement(item, dict, key);
                    index++;
                }
                break;
            default:
                dict[parentKey] = element.ToString();
                break;
        }
    }
}
