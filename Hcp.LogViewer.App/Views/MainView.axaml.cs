// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.App;

internal partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();

        MainContainer.AddHandler(DragDrop.DragEnterEvent, DragEnterHandler);
        MainContainer.AddHandler(DragDrop.DropEvent, DropHandler);
    }

    private void DragEnterHandler(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    public async void DropHandler(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles()?.ToList();
            if (files?.Count > 0)
            {
                var filePath = files[0].Path.LocalPath;
                if (DataContext is MainViewModel vm)
                {
                    var window = TopLevel.GetTopLevel(this) as Window;
                    await vm.OpenFileWithPathAsync((filePath, window!));
                }
            }
        }
        e.Handled = true;
    }
}
