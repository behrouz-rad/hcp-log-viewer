// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using Hcp.LogViewer.App.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal sealed class OpenFileWithPathCommand : ICommandBase<(string filePath, Window window), Unit>
{
    private readonly MainViewModel _viewModel;

    public string Name => "OpenFileWithPath";

    public ReactiveCommand<(string filePath, Window window), Unit> Command { get; }

    public OpenFileWithPathCommand(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        Command = ReactiveCommand.CreateFromTask<(string filePath, Window window)>(ExecuteAsync);
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
