// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Rad.LogViewer.App.ViewModels;

namespace Rad.LogViewer.App.Views;

internal sealed partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = new AboutViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
