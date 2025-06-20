// © 2025 Behrouz Rad. All rights reserved.

using Avalonia.Platform.Storage;
using Hcp.LogViewer.App.Services.Dialogs;

namespace Hcp.LogViewer.Tests.Services.Dialogs;

public class FileDialogServiceTests
{
    private class TestableFileDialogService(IStorageProvider storageProvider, Func<IStorageFile, string?> tryGetLocalPathFunc) : FileDialogService
    {
        protected override IStorageProvider GetStorageProvider()
        {
            return storageProvider;
        }

        protected override string? GetLocalPath(IStorageFile file)
        {
            return tryGetLocalPathFunc(file);
        }
    }

    [Fact]
    public async Task ShowOpenFileDialogAsync_WhenFileSelected_ShouldReturnFilePath()
    {
        // Arrange
        const string expectedPath = "C:\\test\\file.log";
        var mockFile = Mock.Of<IStorageFile>();

        var mockStorageProvider = new Mock<IStorageProvider>();
        mockStorageProvider
            .Setup(p => p.OpenFilePickerAsync(It.IsAny<FilePickerOpenOptions>()))
            .ReturnsAsync([mockFile]);

        var fileDialogService = new TestableFileDialogService(
             mockStorageProvider.Object,
             _ => expectedPath);

        // Act
        var result = await fileDialogService.ShowOpenFileDialogAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedPath);
    }

    [Fact]
    public async Task ShowOpenFileDialogAsync_WhenCancelled_ShouldReturnFailure()
    {
        // Arrange
        var mockStorageProvider = new Mock<IStorageProvider>();
        mockStorageProvider
            .Setup(p => p.OpenFilePickerAsync(It.IsAny<FilePickerOpenOptions>()))
            .ReturnsAsync([]);

        var fileDialogService = new TestableFileDialogService(mockStorageProvider.Object, _ => null);

        // Act
        var result = await fileDialogService.ShowOpenFileDialogAsync();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Operation cancelled");
    }

    [Fact]
    public async Task ShowSaveFileDialogAsync_WhenFileSelected_ShouldReturnFilePath()
    {
        // Arrange
        const string expectedPath = "C:\\test\\export.csv";
        var mockFile = new Mock<IStorageFile>();

        var mockStorageProvider = new Mock<IStorageProvider>();
        mockStorageProvider
            .Setup(p => p.SaveFilePickerAsync(It.IsAny<FilePickerSaveOptions>()))
            .ReturnsAsync(mockFile.Object);

        var fileDialogService = new TestableFileDialogService(mockStorageProvider.Object, _ => expectedPath);

        // Act
        var result = await fileDialogService.ShowSaveFileDialogAsync(".csv", "export");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedPath);
    }

    [Fact]
    public async Task ShowSaveFileDialogAsync_WhenCancelled_ShouldReturnFailure()
    {
        // Arrange
        var mockStorageProvider = new Mock<IStorageProvider>();
        mockStorageProvider
            .Setup(p => p.SaveFilePickerAsync(It.IsAny<FilePickerSaveOptions>()))
            .ReturnsAsync((IStorageFile)null!);

        var fileDialogService = new TestableFileDialogService(mockStorageProvider.Object, _ => null);

        // Act
        var result = await fileDialogService.ShowSaveFileDialogAsync(".csv");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Operation cancelled");
    }
}
