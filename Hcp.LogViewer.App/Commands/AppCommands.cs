// © 2025 Behrouz Rad. All rights reserved.

using Hcp.LogViewer.App.Commands.Implementations;

namespace Hcp.LogViewer.App.Commands;

/// <summary>
/// Centralized command registry for the application
/// </summary>
internal sealed class AppCommands(
    OpenFileCommand openFile,
    OpenFileWithPathCommand openFileWithPath,
    ExportToCsvCommand exportToCsv,
    ExitCommand exit,
    ClearSearchCommand clearSearch,
    ShowAboutCommand showAbout,
    CopyLogEntryCommand copyLogEntry,
    ToggleThemeCommand toggleTheme)
{
    public OpenFileCommand OpenFile { get; } = openFile;
    public OpenFileWithPathCommand OpenFileWithPath { get; } = openFileWithPath;
    public ExportToCsvCommand ExportToCsv { get; } = exportToCsv;
    public ExitCommand Exit { get; } = exit;
    public ClearSearchCommand ClearSearch { get; } = clearSearch;
    public ShowAboutCommand ShowAbout { get; } = showAbout;
    public CopyLogEntryCommand CopyLogEntry { get; } = copyLogEntry;
    public ToggleThemeCommand ToggleTheme { get; } = toggleTheme;
}
