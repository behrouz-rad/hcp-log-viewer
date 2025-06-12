// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Hcp.LogViewer.App.ViewModels;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class ClearSearchCommand : CommandBase<Unit>
{
    private readonly MainViewModel _viewModel;
    private readonly ReactiveCommand<Unit, Unit> _command;

    public override string Name => "ClearSearch";

    public override ReactiveCommand<Unit, Unit> Command => _command;

    public ClearSearchCommand(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        _command = ReactiveCommand.Create(Execute);
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
