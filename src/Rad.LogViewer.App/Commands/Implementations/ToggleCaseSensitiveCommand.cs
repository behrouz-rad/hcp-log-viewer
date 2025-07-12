// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Rad.LogViewer.App.Commands;
using Rad.LogViewer.App.Constants;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands.Implementations;

/// <summary>
/// Command to toggle case sensitivity for search operations
/// </summary>
internal class ToggleCaseSensitiveCommand : ICommandBase
{
    private readonly MainViewModel _viewModel;
    private readonly ISettingsService _settingsService;

    public ToggleCaseSensitiveCommand(MainViewModel viewModel, ISettingsService settingsService)
    {
        _viewModel = viewModel;
        _settingsService = settingsService;

        Command = ReactiveCommand.CreateFromTask(Execute);
    }

    public ReactiveCommand<Unit, Unit> Command { get; }

    public string Name => "ToggleCaseSensitive";

    private async Task Execute()
    {
        _viewModel.IsCaseSensitive = !_viewModel.IsCaseSensitive;
        await _settingsService.SaveSettingAsync(AppConstants.Settings.IsCaseSensitive, _viewModel.IsCaseSensitive);
    }
}