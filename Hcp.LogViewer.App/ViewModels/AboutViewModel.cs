using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reflection;

namespace Hcp.LogViewer.App.ViewModels;

internal sealed class AboutViewModel : ViewModelBase
{
    public string Version { get; init; }
    public string Copyright { get; init; }
    
    public ReactiveCommand<Window, Unit> CloseCommand { get; }

    public AboutViewModel()
    {
        var assembly = Assembly.GetExecutingAssembly();
        Version = assembly.GetName().Version?.ToString() ?? "1.0.0";
        
        Copyright = $"© {DateTime.Now.Year} SnkeOS";
        
        CloseCommand = ReactiveCommand.Create<Window>(CloseWindow);
    }

    private static void CloseWindow(Window window) => window.Close();
}
