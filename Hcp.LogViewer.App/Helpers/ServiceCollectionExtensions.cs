using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Hcp.LogViewer.App.Helpers;

internal static class ServiceCollectionExtensions
{
  public static void AddCommonServices(this IServiceCollection collection)
  {
    collection.AddTransient<ILogFileParser, LogFileParser>();
    collection.AddTransient<IFileDialogService, FileDialogService>();
    collection.AddTransient<IJsonToCsvConverter, JsonToCsvConverter>();
    collection.AddTransient<MainViewModel>();
  }
}
