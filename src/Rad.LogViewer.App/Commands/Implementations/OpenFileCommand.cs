// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using Rad.LogViewer.App.Commands;
using Rad.LogViewer.App.Services.Dialogs;
using Rad.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands.Implementations;

internal class OpenFileCommand : ICommandBase<Window, Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly IFileDialogService _fileDialogService;

    public string Name => "OpenFile";

    public ReactiveCommand<Window, Unit> Command { get; }

    public OpenFileCommand(MainViewModel viewModel, IFileDialogService fileDialogService)
    {
        _viewModel = viewModel;
        _fileDialogService = fileDialogService;
        Command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    protected internal virtual async Task ExecuteAsync(Window window)
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