// © 2025 Behrouz Rad. All rights reserved.

namespace Hcp.LogViewer.App.Services.Converters;

/// <summary>
/// Defines methods for converting JSON log files to CSV format.
/// </summary>
/// <remarks>
/// Implementations of this interface should handle the conversion of structured JSON logs
/// to a flattened CSV format suitable for analysis in spreadsheet applications.
/// </remarks>
internal interface IJsonToCsvConverter
{
    /// <summary>
    /// Converts a JSON log file to CSV format.
    /// </summary>
    /// <param name="jsonFilePath">Path to the source JSON log file.</param>
    /// <param name="csvFilePath">Path where the CSV file will be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ConvertAsync(string jsonFilePath, string csvFilePath, CancellationToken cancellationToken = default);
}
