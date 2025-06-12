// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class OpenFileCommand : CommandBase<Window, Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly IFileDialogService _fileDialogService;
    private readonly ReactiveCommand<Window, Unit> _command;

    public override string Name => "OpenFile";

    public override ReactiveCommand<Window, Unit> Command => _command;

    public OpenFileCommand(MainViewModel viewModel, IFileDialogService fileDialogService)
    {
        _viewModel = viewModel;
        _fileDialogService = fileDialogService;
        _command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    private async Task ExecuteAsync(Window window)
    {
        _viewModel.CancelPreviousOperation();

        var filePathResult = await _fileDialogService.ShowOpenFileDialogAsync();
        if (filePathResult.IsFailed)
        {
            return;
        }

        var filePath = filePathResult.Value;
        await _viewModel.OpenFileWithPathAsync((filePath, window));
    }
}
