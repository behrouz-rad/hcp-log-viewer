﻿// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.App.Views;

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
