﻿// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Rad.LogViewer.App.Commands;
using Rad.LogViewer.App.Services.Converters;
using Rad.LogViewer.App.Services.Dialogs;
using Rad.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands.Implementations;

internal class ExportToCsvCommand : ICommandBase<Window, Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly IFileDialogService _fileDialogService;
    private readonly IJsonToCsvConverter _jsonToCsvConverter;

    public string Name => "ExportToCsv";

    public ReactiveCommand<Window, Unit> Command { get; }

    public ExportToCsvCommand(MainViewModel viewModel, IFileDialogService fileDialogService, IJsonToCsvConverter jsonToCsvConverter)
    {
        _viewModel = viewModel;
        _fileDialogService = fileDialogService;
        _jsonToCsvConverter = jsonToCsvConverter;
        Command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    protected internal virtual async Task ExecuteAsync(Window window)
    {
        if (!_viewModel.IsLoaded)
        {
            // Skip showing message box in tests
            if (window != null)
            {
                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Export to CSV",
                    "Please open a log file first.",
                    ButtonEnum.Ok,
                    Icon.Warning);

                await msgBox.ShowWindowDialogAsync(window);
            }
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

            // Skip showing message box in tests
            if (window != null)
            {
                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Export to CSV",
                    $"Successfully exported to {csvFilePath}",
                    ButtonEnum.Ok,
                    Icon.Info);

                await msgBox.ShowWindowDialogAsync(window);
            }
        }
        catch (Exception ex)
        {
            // Skip showing message box in tests
            if (window != null)
            {
                var msgBox = MessageBoxManager.GetMessageBoxStandard(
                    "Export to CSV",
                    $"Error exporting to CSV: {ex.Message}",
                    ButtonEnum.Ok,
                    Icon.Error);

                await msgBox.ShowWindowDialogAsync(window);
            }
        }
        finally
        {
            _viewModel.IsLoading = false;
        }
    }
}