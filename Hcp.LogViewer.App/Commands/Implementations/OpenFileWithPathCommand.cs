// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class OpenFileWithPathCommand : CommandBase<(string filePath, Window window), Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly ILogFileParser _logFileParser;
    private readonly ReactiveCommand<(string filePath, Window window), Unit> _command;

    public override string Name => "OpenFileWithPath";

    public override ReactiveCommand<(string filePath, Window window), Unit> Command => _command;

    public OpenFileWithPathCommand(MainViewModel viewModel, ILogFileParser logFileParser)
    {
        _viewModel = viewModel;
        _logFileParser = logFileParser;
        _command = ReactiveCommand.CreateFromTask<(string filePath, Window window)>(ExecuteAsync);
    }

    private async Task ExecuteAsync((string filePath, Window window) param)
    {
        _viewModel.IsLoading = true;
        _viewModel.CancelPreviousOperation();
        _viewModel.ClearSourceEntries();

        try
        {
            await _viewModel.LoadLogEntriesAsync(param.filePath, _viewModel.CancellationToken);
            _viewModel.SelectedFilePath = param.filePath;
            _viewModel.IsLoaded = true;
        }
        catch (OperationCanceledException)
        {
            _viewModel.SelectedFilePath = $"Loading cancelled for {param.filePath}";

            var msgBox = MessageBoxManager.GetMessageBoxStandard("HCP Log Viewer", $"Loading cancelled for {param.filePath}",
                                              ButtonEnum.Ok, Icon.Warning);

            await msgBox.ShowWindowDialogAsync(param.window);
        }
        catch (Exception ex)
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard("HCP Log Viewer", $"Error loading file: {ex.Message}",
                                              ButtonEnum.Ok, Icon.Error);

            await msgBox.ShowWindowDialogAsync(param.window);
        }
        finally
        {
            _viewModel.IsLoading = false;
        }
    }
}
