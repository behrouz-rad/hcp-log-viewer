// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Rad.LogViewer.App.Commands;
using Rad.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Rad.LogViewer.App.Commands.Implementations;

internal sealed class ClearSearchCommand : ICommandBase<Unit>
{
    private readonly MainViewModel _viewModel;

    public string Name => "ClearSearch";

    public ReactiveCommand<Unit, Unit> Command { get; }

    public ClearSearchCommand(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        Command = ReactiveCommand.Create(Execute);
    }

    private void Execute()
    {
        _viewModel.SearchAllText = null;
        _viewModel.MessageSearchText = null;
        _viewModel.LogLevel = null;
        _viewModel.AttributesSearchText = null;
        _viewModel.DateSearch = null;
    }
}
