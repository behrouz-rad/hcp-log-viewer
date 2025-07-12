// © 2025 Behrouz Rad. All rights reserved.

using Rad.LogViewer.App.Commands.Implementations;
using Rad.LogViewer.App.Services.Converters;
using Rad.LogViewer.App.Services.Dialogs;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.Services.Theme;
using Rad.LogViewer.App.ViewModels;

namespace Rad.LogViewer.App.Commands;

/// <summary>
/// Factory for creating command instances
/// </summary>
internal static class CommandFactory
{
    /// <summary>
    /// Creates all application commands for the given view model
    /// </summary>
    public static AppCommands CreateCommands(
        MainViewModel viewModel,
        IFileDialogService fileDialogService,
        IJsonToCsvConverter jsonToCsvConverter,
        IThemeService themeService,
        ISettingsService settingsService)
    {
        return new AppCommands(
            new OpenFileCommand(viewModel, fileDialogService),
            new OpenFileWithPathCommand(viewModel),
            new ExportToCsvCommand(viewModel, fileDialogService, jsonToCsvConverter),
            new ExitCommand(),
            new ClearSearchCommand(viewModel),
            new ShowAboutCommand(),
            new CopyLogEntryCommand(),
            new ToggleThemeCommand(viewModel, themeService),
            new TogglePropertyTitlesCommand(viewModel, settingsService),
            new ToggleCaseSensitiveCommand(viewModel, settingsService)
        );
    }
}
