// © 2025 Behrouz Rad. All rights reserved.

using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.Services.Theme;
using Hcp.LogViewer.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Hcp.LogViewer.App.Helpers;

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
        collection.AddSingleton<IThemeService, ThemeService>();

        collection.AddTransient<MainViewModel>();
    }
}
