// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;

namespace Rad.LogViewer.App.Services.Dialogs;

/// <summary>
/// Defines methods for displaying file open and save dialogs.
/// </summary>
internal interface IFileDialogService
{
    /// <summary>
    /// Shows a file open dialog to select a log file.
    /// </summary>
    /// <returns>A result containing the selected file path or an error if canceled.</returns>
    public Task<Result<string>> ShowOpenFileDialogAsync();

    /// <summary>
    /// Shows a file save dialog to save a file.
    /// </summary>
    /// <param name="defaultExtension">The default file extension.</param>
    /// <param name="initialFileName">Optional initial file name suggestion.</param>
    /// <returns>A result containing the selected save path or an error if canceled.</returns>
    public Task<Result<string>> ShowSaveFileDialogAsync(string defaultExtension, string? initialFileName = null);
}
