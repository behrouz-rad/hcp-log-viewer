// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Rad.LogViewer.App.Commands;
using Rad.LogViewer.App.Services.Theme;
using Rad.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands.Implementations;

internal sealed class ToggleThemeCommand : ICommandBase<Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly IThemeService _themeService;

    public string Name => "ToggleTheme";

    public ReactiveCommand<Unit, Unit> Command { get; }

    public ToggleThemeCommand(MainViewModel viewModel, IThemeService themeService)
    {
        _viewModel = viewModel;
        _themeService = themeService;
        Command = ReactiveCommand.Create(Execute);
    }

    private void Execute()
    {
        _themeService.ToggleTheme();
        _viewModel.RaisePropertyChanged(nameof(MainViewModel.IsDarkTheme));
    }
}
