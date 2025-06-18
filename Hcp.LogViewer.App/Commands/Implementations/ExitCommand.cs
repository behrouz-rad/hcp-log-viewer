// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal sealed class ExitCommand : ICommandBase<Unit>
{
    public string Name => "Exit";

    public ReactiveCommand<Unit, Unit> Command { get; }

    public ExitCommand()
    {
        Command = ReactiveCommand.Create(Execute);
    }

    private static void Execute()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}
