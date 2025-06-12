using System.Threading;
using System.Threading.Tasks;

namespace Hcp.LogViewer.App.Services.Converters;

public interface IJsonToCsvConverter
{
    Task ConvertAsync(string jsonFilePath, string csvFilePath, CancellationToken cancellationToken = default);
}
