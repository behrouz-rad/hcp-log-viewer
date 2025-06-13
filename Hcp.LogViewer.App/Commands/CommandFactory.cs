// © 2025 Behrouz Rad. All rights reserved.

using Hcp.LogViewer.App.Commands.Implementations;
using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.App.Commands;

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
        IJsonToCsvConverter jsonToCsvConverter)
    {
        return new AppCommands(
            new OpenFileCommand(viewModel, fileDialogService),
            new OpenFileWithPathCommand(viewModel),
            new ExportToCsvCommand(viewModel, fileDialogService, jsonToCsvConverter),
            new ExitCommand(),
            new ClearSearchCommand(viewModel),
            new ShowAboutCommand(),
            new CopyLogEntryCommand()
        );
    }
}
