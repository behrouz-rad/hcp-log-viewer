using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using FluentResults;

namespace Hcp.LogViewer.App.Services.Dialogs;

internal sealed class FileDialogService() : IFileDialogService
{
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
            var filePath = files[0].TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Fail("Could not get file path");
            }

            return Result.Ok(filePath);
        }

        return Result.Fail("Operation cancelled");
    }

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
            var filePath = file.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Fail("Could not get file path");
            }

            return Result.Ok(filePath);
        }

        return Result.Fail("Operation cancelled");
    }

    private static IStorageProvider GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
        {
            throw new NullReferenceException("Missing StorageProvider instance.");
        }

        return provider;
    }
}
