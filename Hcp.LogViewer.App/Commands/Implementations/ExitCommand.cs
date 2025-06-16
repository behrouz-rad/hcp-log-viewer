// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class ExitCommand : ICommandBase<Unit>
{
    private readonly ReactiveCommand<Unit, Unit> _command;

    public string Name => "Exit";

    public ReactiveCommand<Unit, Unit> Command => _command;

    public ExitCommand()
    {
        _command = ReactiveCommand.Create(Execute);
    }

    private static void Execute()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
