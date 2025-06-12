// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.App;

internal class ViewLocator : IDataTemplate
{

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
