// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive;
using System.Reflection;
using Avalonia.Controls;
using ReactiveUI;

namespace Rad.LogViewer.App.ViewModels;

/// <summary>
/// View model for the About dialog.
/// </summary>
internal sealed class AboutViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the application version.
    /// </summary>
    public string Version { get; init; }

    /// <summary>
    /// Gets the copyright information.
    /// </summary>
    public string Copyright { get; init; }

    /// <summary>
    /// Gets the command to close the About dialog.
    /// </summary>
    public ReactiveCommand<Window, Unit> CloseCommand { get; }

    /// <summary>
    /// Initializes a new instance of the AboutViewModel class.
    /// </summary>
    public AboutViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        Version = assembly.GetName().Version?.ToString() ?? "1.0.0";

        Copyright = $"© {DateTime.Now.Year} SnkeOS";

        CloseCommand = ReactiveCommand.Create<Window>(CloseWindow);
    }

    /// <summary>
    /// Closes the specified window.
    /// </summary>
    /// <param name="window">The window to close.</param>
    private static void CloseWindow(Window window) => window.Close();
}
