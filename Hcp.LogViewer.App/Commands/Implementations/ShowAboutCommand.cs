// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace Hcp.LogViewer.App.Commands.Implementations;

internal sealed class ShowAboutCommand : ICommandBase<Window, Unit>
{
    public string Name => "ShowAbout";

    public ReactiveCommand<Window, Unit> Command { get; }

    public ShowAboutCommand()
    {
        Command = ReactiveCommand.CreateFromTask<Window>(ExecuteAsync);
    }

    private static Task ExecuteAsync(Window owner)
    {
        var aboutWindow = new Views.AboutWindow();
        return aboutWindow.ShowDialog(owner);
    }
}
