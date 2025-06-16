// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class ExportToCsvCommand : ICommandBase<Window, Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly IFileDialogService _fileDialogService;
    private readonly IJsonToCsvConverter _jsonToCsvConverter;
    private readonly ReactiveCommand<Window, Unit> _command;

    public string Name => "ExportToCsv";

    public ReactiveCommand<Window, Unit> Command => _command;

    public ExportToCsvCommand(MainViewModel viewModel, IFileDialogService fileDialogService, IJsonToCsvConverter jsonToCsvConverter)
    {
        _viewModel = viewModel;
        _fileDialogService = fileDialogService;
        _jsonToCsvConverter = jsonToCsvConverter;
        _command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    private async Task ExecuteAsync(Window window)
    {
        if (!_viewModel.IsLoaded)
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Export to CSV",
                "Please open a log file first.",
                ButtonEnum.Ok,
                Icon.Warning);

            await msgBox.ShowWindowDialogAsync(window);
            return;
        }

        var filePathResult = await _fileDialogService.ShowSaveFileDialogAsync(".csv",
            Path.GetFileNameWithoutExtension(_viewModel.SelectedFilePath) + ".csv");

        if (filePathResult.IsFailed)
        {
            return;
        }

        var csvFilePath = filePathResult.Value;

        try
        {
            _viewModel.CancelPreviousOperation();
            _viewModel.IsLoading = true;

            await _jsonToCsvConverter.ConvertAsync(_viewModel.SelectedFilePath, csvFilePath, _viewModel.CancellationToken);

            _viewModel.IsLoading = false;

            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Export to CSV",
                $"Successfully exported to {csvFilePath}",
                ButtonEnum.Ok,
                Icon.Info);

            await msgBox.ShowWindowDialogAsync(window);
        }
        catch (Exception ex)
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                "Export to CSV",
                $"Error exporting to CSV: {ex.Message}",
                ButtonEnum.Ok,
                Icon.Error);

            await msgBox.ShowWindowDialogAsync(window);
        }
        finally
        {
            _viewModel.IsLoading = false;
        }
    }
}
