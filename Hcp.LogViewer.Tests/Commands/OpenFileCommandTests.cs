// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Controls;
using FluentResults;
using Hcp.LogViewer.App.Commands.Implementations;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.Tests.Commands;

public class OpenFileCommandTests
{
    [Fact]
    public async Task Execute_WhenFileSelected_ShouldOpenFile()
    {
        // Arrange
        var mockFileDialogService = new Mock<IFileDialogService>();
        mockFileDialogService
            .Setup(s => s.ShowOpenFileDialogAsync())
            .ReturnsAsync(Result.Ok("C:\\test\\file.log"));

        var mockViewModel = new Mock<MainViewModel>();
        mockViewModel.Setup(vm => vm.OpenFileWithPathAsync(It.IsAny<(string, Window)>()))
            .Returns(Task.CompletedTask);

        var command = new OpenFileCommand(mockViewModel.Object, mockFileDialogService.Object);

        // Act
        await command.ExecuteAsync(null!);

        // Assert
        mockViewModel.Verify(vm => vm.CancelPreviousOperation(), Times.Once);
        mockViewModel.Verify(vm => vm.OpenFileWithPathAsync(It.Is<(string, Window)>(
            param => param.Item1 == "C:\\test\\file.log")),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WhenCancelled_ShouldNotOpenFile()
    {
        // Arrange
        var mockFileDialogService = new Mock<IFileDialogService>();
        mockFileDialogService
            .Setup(s => s.ShowOpenFileDialogAsync())
            .ReturnsAsync(Result.Fail("Operation cancelled"));

        var mockViewModel = new Mock<MainViewModel>();
        var command = new OpenFileCommand(mockViewModel.Object, mockFileDialogService.Object);

        // Act - directly call the protected ExecuteAsync method
        await command.ExecuteAsync(null!);

        // Assert
        mockViewModel.Verify(vm => vm.CancelPreviousOperation(), Times.Once);
        mockViewModel.Verify(vm => vm.OpenFileWithPathAsync(It.IsAny<(string, Window)>()), Times.Never);
    }
}
