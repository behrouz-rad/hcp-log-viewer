// © 2025 Behrouz Rad. All rights reserved.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Rad.LogViewer.App.Views;
using Microsoft.Extensions.DependencyInjection;
using Rad.LogViewer.App.Helpers;
using Rad.LogViewer.App.Services.Theme;
using Rad.LogViewer.App.ViewModels;

namespace Rad.LogViewer.App;

internal partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        var services = collection.BuildServiceProvider();

        var themeService = services.GetRequiredService<IThemeService>();
        themeService.ApplyTheme();

        var vm = services.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow()
            {
                DataContext = vm
            };

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
