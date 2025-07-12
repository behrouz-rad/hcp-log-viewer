// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Rad.LogViewer.App.Services.Converters;
using Rad.LogViewer.App.Services.Dialogs;
using Rad.LogViewer.App.Services.Parsers;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.Services.Theme;
using Rad.LogViewer.App.ViewModels;

namespace Rad.LogViewer.App.Helpers;

/// <summary>
/// Extension methods for configuring services in the application.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers common application services with the dependency injection container.
    /// </summary>
    /// <param name="collection">The service collection to add services to.</param>
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<ILogFileParser, LogFileParser>();
        collection.AddTransient<IFileDialogService, FileDialogService>();
        collection.AddTransient<IJsonToCsvConverter, JsonToCsvConverter>();
        collection.AddSingleton<ISettingsService, SettingsService>();
        collection.AddSingleton<IThemeService, ThemeService>();

        collection.AddTransient<MainViewModel>();
    }
}
