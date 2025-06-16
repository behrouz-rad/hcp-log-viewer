// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal class ShowAboutCommand : ICommandBase<Window, Unit>
{
    private readonly ReactiveCommand<Window, Unit> _command;

    public string Name => "ShowAbout";

    public ReactiveCommand<Window, Unit> Command => _command;

    public ShowAboutCommand()
    {
        _command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    private static async Task ExecuteAsync(Window owner)
    {
        var aboutWindow = new Views.AboutWindow();
        await aboutWindow.ShowDialog(owner);
    }
}
