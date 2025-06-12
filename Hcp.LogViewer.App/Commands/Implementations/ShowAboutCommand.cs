// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class ShowAboutCommand : CommandBase<Window, Unit>
{
    private readonly ReactiveCommand<Window, Unit> _command;

    public override string Name => "ShowAbout";

    public override ReactiveCommand<Window, Unit> Command => _command;

    public ShowAboutCommand()
    {
        _command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    private async Task ExecuteAsync(Window owner)
    {
        var aboutWindow = new Views.AboutWindow();
        await aboutWindow.ShowDialog(owner);
    }
}
