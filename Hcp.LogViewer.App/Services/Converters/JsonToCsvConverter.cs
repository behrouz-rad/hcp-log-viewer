// © 2025 Behrouz Rad. All rights reserved.

using System.Text;
using System.Text.Json;

namespace Hcp.LogViewer.App.Services.Converters;

/// <summary>
/// Converts JSON log files to CSV format.
/// </summary>
/// <remarks>
/// This class handles the conversion of structured JSON logs to a flattened CSV format
/// suitable for analysis in spreadsheet applications. Nested JSON objects are flattened
/// </remarks>
public class JsonToCsvConverter : IJsonToCsvConverter
{
    /// <summary>
    /// Converts a JSON log file to CSV format.
    /// </summary>
    /// <param name="jsonFilePath">Path to the source JSON log file.</param>
    /// <param name="csvFilePath">Path where the CSV file will be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ConvertAsync(string jsonFilePath, string csvFilePath, CancellationToken cancellationToken = default)
    {
        var headers = await CollectHeadersAsync(jsonFilePath, cancellationToken);

        await WriteCsvAsync(jsonFilePath, csvFilePath, headers, cancellationToken);
    }

    /// <summary>
    /// Collects all unique headers from the JSON file.
    /// </summary>
    /// <param name="jsonFilePath">Path to the JSON file.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A set of unique header names found in the JSON file.</returns>
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

    /// <summary>
    /// Writes the CSV file using the collected headers.
    /// </summary>
    /// <param name="jsonFilePath">Path to the source JSON file.</param>
    /// <param name="csvFilePath">Path where the CSV file will be created.</param>
    /// <param name="headers">The set of headers to include in the CSV.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Escapes a string value for CSV format.
    /// </summary>
    /// <param name="value">The string value to escape.</param>
    /// <returns>The escaped string, surrounded by double quotes.</returns>
    /// <remarks>
    /// This method handles escaping double quotes by doubling them and
    /// ensures all values are enclosed in double quotes.
    /// </remarks>
    private static string EscapeCsv(string? value)
    {
        // Escape double quotes by doubling them
        var escaped = value?.Replace("\"", "\"\"");

        return $"\"{escaped ?? ""}\"";
    }

    /// <summary>
    /// Flattens a JSON element into a dictionary with dot notation for nested objects.
    /// </summary>
    /// <param name="element">The JSON element to flatten.</param>
    /// <param name="dict">The dictionary to populate with flattened values.</param>
    /// <param name="parentKey">The parent key for nested properties.</param>
    /// <remarks>
    /// This method recursively processes JSON objects and arrays:
    /// - Objects: Properties are flattened using dot notation (e.g., "parent.child")
    /// - Arrays: Items are flattened using index notation (e.g., "array[0]")
    /// - Primitive values: Added directly to the dictionary
    /// </remarks>
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
                    string key = $"{parentKey}[{index++}]";
                    FlattenJsonElement(item, dict, key);
                }
                break;
            default:
                dict[parentKey] = element.ToString();
                break;
        }
    }
}
