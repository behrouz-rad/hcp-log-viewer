using FluentResults;
using System.Threading.Tasks;

namespace Hcp.LogViewer.App.Services.Dialogs;

internal interface IFileDialogService
{
  public Task<Result<string>> ShowOpenFileDialogAsync();
  public Task<Result<string>> ShowSaveFileDialogAsync(string defaultExtension, string? initialFileName = null);
}
