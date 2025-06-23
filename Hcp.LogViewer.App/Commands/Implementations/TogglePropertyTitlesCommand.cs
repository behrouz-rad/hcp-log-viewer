// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Hcp.LogViewer.App.Services.Settings;
using Hcp.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

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
        await _settingsService.SaveSettingAsync(Constants.AppConstants.Settings.ShowPropertyTitles, _viewModel.ShowPropertyTitles);
    }
}
