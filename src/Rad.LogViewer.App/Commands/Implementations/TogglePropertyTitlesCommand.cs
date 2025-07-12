// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Rad.LogViewer.App.Commands;
using Rad.LogViewer.App.Constants;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands.Implementations;

/// <summary>
/// Command to toggle the visibility of property titles in log entries
/// </summary>
internal class TogglePropertyTitlesCommand : ICommandBase
{
    private readonly MainViewModel _viewModel;
    private readonly ISettingsService _settingsService;

    public TogglePropertyTitlesCommand(MainViewModel viewModel, ISettingsService settingsService)
    {
        _viewModel = viewModel;
        _settingsService = settingsService;

        Command = ReactiveCommand.CreateFromTask(Execute);
    }

    public ReactiveCommand<Unit, Unit> Command { get; }

    public string Name => "TogglePropertyTitles";

    private async Task Execute()
    {
        _viewModel.ShowPropertyTitles = !_viewModel.ShowPropertyTitles;
        await _settingsService.SaveSettingAsync(AppConstants.Settings.ShowPropertyTitles, _viewModel.ShowPropertyTitles);
    }
}
