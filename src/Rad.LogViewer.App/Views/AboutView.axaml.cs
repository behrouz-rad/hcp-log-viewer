// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rad.LogViewer.App.Views;

internal sealed partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
