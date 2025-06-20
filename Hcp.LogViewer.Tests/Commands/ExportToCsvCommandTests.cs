// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using Hcp.LogViewer.App.Commands.Implementations;
using Hcp.LogViewer.App.Services.Converters;
using Hcp.LogViewer.App.Services.Dialogs;
using Hcp.LogViewer.App.Services.Parsers;
using Hcp.LogViewer.App.Services.Theme;
using Hcp.LogViewer.App.ViewModels;

namespace Hcp.LogViewer.Tests.Commands;

public class ExportToCsvCommandTests
{
    [Fact]
    public async Task Execute_WhenFileSelected_ShouldExportToCsv()
    {
        // Arrange
        var mockFileDialogService = new Mock<IFileDialogService>();
        mockFileDialogService
            .Setup(s => s.ShowSaveFileDialogAsync(".csv", It.IsAny<string>()))
            .ReturnsAsync(Result.Ok("C:\\test\\export.csv"));

        var mockJsonToCsvConverter = new Mock<IJsonToCsvConverter>();
        mockJsonToCsvConverter
            .Setup(c => c.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockLogFileParser = new Mock<ILogFileParser>();
        var themeService = Mock.Of<ThemeService>();

        using var viewModel = new MainViewModel(mockLogFileParser.Object, mockFileDialogService.Object, mockJsonToCsvConverter.Object, themeService);
        viewModel.IsLoaded = true;
        viewModel.SelectedFilePath = "C:\\test\\file.log";

        var command = new ExportToCsvCommand(viewModel, mockFileDialogService.Object, mockJsonToCsvConverter.Object);

        // Act
        await command.ExecuteAsync(null!);

        // Assert
        mockJsonToCsvConverter.Verify(c => c.ConvertAsync(
            "C:\\test\\file.log",
            "C:\\test\\export.csv",
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WhenNoFileLoaded_ShouldNotExport()
    {
        // Arrange
        var mockFileDialogService = new Mock<IFileDialogService>();
        var mockJsonToCsvConverter = new Mock<IJsonToCsvConverter>();
        var mockLogFileParser = new Mock<ILogFileParser>();
        var themeService = Mock.Of<ThemeService>();

        using var viewModel = new MainViewModel(mockLogFileParser.Object, mockFileDialogService.Object, mockJsonToCsvConverter.Object, themeService);
        viewModel.IsLoaded = false;

        var command = new ExportToCsvCommand(viewModel, mockFileDialogService.Object, mockJsonToCsvConverter.Object);

        // Act
        await command.ExecuteAsync(null!);

        // Assert
        mockFileDialogService.Verify(s => s.ShowSaveFileDialogAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockJsonToCsvConverter.Verify(c => c.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Execute_WhenSaveDialogCancelled_ShouldNotExport()
    {
        // Arrange
        var mockFileDialogService = new Mock<IFileDialogService>();
        mockFileDialogService
            .Setup(s => s.ShowSaveFileDialogAsync(".csv", It.IsAny<string>()))
            .ReturnsAsync(Result.Fail("Operation cancelled"));

        var mockJsonToCsvConverter = new Mock<IJsonToCsvConverter>();
        var mockLogFileParser = new Mock<ILogFileParser>();
        var themeService = Mock.Of<ThemeService>();

        using var viewModel = new MainViewModel(mockLogFileParser.Object, mockFileDialogService.Object, mockJsonToCsvConverter.Object, themeService);
        viewModel.IsLoaded = true;
        viewModel.SelectedFilePath = "C:\\test\\file.log";

        var command = new ExportToCsvCommand(viewModel, mockFileDialogService.Object, mockJsonToCsvConverter.Object);

        // Act
        await command.ExecuteAsync(null!);

        // Assert
        mockJsonToCsvConverter.Verify(c => c.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
