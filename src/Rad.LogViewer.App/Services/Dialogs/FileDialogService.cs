// © 2025 Behrouz Rad. All rights reserved.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using FluentResults;

namespace Rad.LogViewer.App.Services.Dialogs;

/// <summary>
/// Implementation of file dialog service.
/// </summary>
internal class FileDialogService : IFileDialogService
{
    /// <summary>
    /// Shows a file open dialog to select a log file.
    /// </summary>
    /// <returns>A result containing the selected file path or an error if canceled.</returns>
    public async Task<Result<string>> ShowOpenFileDialogAsync()
    {
        var files = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Log File",
            AllowMultiple = false,
            FileTypeFilter = [
                              new FilePickerFileType("Log Files") { Patterns = ["*.log", "*.txt"] },
                              new FilePickerFileType("All Files") { Patterns = ["*.*"] }
                           ]
        });

        if (files.Count >= 1)
        {
            var filePath = GetLocalPath(files[0]);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Fail("Could not get file path");
            }

            return Result.Ok(filePath);
        }

        return Result.Fail("Operation cancelled");
    }

    /// <summary>
    /// Shows a file save dialog to save a file.
    /// </summary>
    /// <param name="defaultExtension">The default file extension.</param>
    /// <param name="initialFileName">Optional initial file name suggestion.</param>
    /// <returns>A result containing the selected save path or an error if canceled.</returns>
    public async Task<Result<string>> ShowSaveFileDialogAsync(string defaultExtension, string? initialFileName = null)
    {
        var fileTypes = defaultExtension.ToLowerInvariant() switch
        {
            ".csv" => [new FilePickerFileType("CSV Files") { Patterns = ["*.csv"] }],
            _ => new[] { new FilePickerFileType("All Files") { Patterns = ["*.*"] } }
        };

        var file = await GetStorageProvider().SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Save {defaultExtension.TrimStart('.')} File",
            DefaultExtension = defaultExtension,
            SuggestedFileName = initialFileName,
            FileTypeChoices = fileTypes
        });

        if (file != null)
        {
            var filePath = GetLocalPath(file);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Fail("Could not get file path");
            }

            return Result.Ok(filePath);
        }

        return Result.Fail("Operation cancelled");
    }

    /// <summary>
    /// Gets the storage provider from the current application.
    /// </summary>
    /// <returns>The storage provider for file operations.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the storage provider cannot be found.</exception>
    protected virtual IStorageProvider GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
        {
            throw new InvalidOperationException("Missing StorageProvider instance.");
        }

        return provider;
    }

    protected virtual string? GetLocalPath(IStorageFile file)
    {
        return file.TryGetLocalPath();
    }
}
