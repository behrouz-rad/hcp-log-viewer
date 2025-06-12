// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class ExitCommand : CommandBase<Unit>
{
    private readonly ReactiveCommand<Unit, Unit> _command;

    public override string Name => "Exit";

    public override ReactiveCommand<Unit, Unit> Command => _command;

    public ExitCommand()
    {
        _command = ReactiveCommand.Create(Execute);
    }

    private void Execute()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
