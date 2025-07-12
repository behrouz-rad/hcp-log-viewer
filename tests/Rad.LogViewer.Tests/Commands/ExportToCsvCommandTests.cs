// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using Rad.LogViewer.App.Commands.Implementations;
using Rad.LogViewer.App.Services.Converters;
using Rad.LogViewer.App.Services.Dialogs;
using Rad.LogViewer.App.Services.Parsers;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.Services.Theme;
using Rad.LogViewer.App.ViewModels;

namespace Rad.LogViewer.Tests.Commands;

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
        var themeService = Mock.Of<IThemeService>();
        var settingsService = Mock.Of<ISettingsService>();

        using var viewModel = new MainViewModel(mockLogFileParser.Object,
                                                mockFileDialogService.Object,
                                                mockJsonToCsvConverter.Object,
                                                themeService,
                                                settingsService);
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
        var themeService = Mock.Of<IThemeService>();
        var settingsService = Mock.Of<ISettingsService>();

        using var viewModel = new MainViewModel(mockLogFileParser.Object,
                                                mockFileDialogService.Object,
                                                mockJsonToCsvConverter.Object,
                                                themeService, settingsService);
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
        var themeService = Mock.Of<IThemeService>();
        var settingsService = Mock.Of<ISettingsService>();

        using var viewModel = new MainViewModel(mockLogFileParser.Object,
                                                mockFileDialogService.Object,
                                                mockJsonToCsvConverter.Object,
                                                themeService,
                                                settingsService);
        viewModel.IsLoaded = true;
        viewModel.SelectedFilePath = "C:\\test\\file.log";

        var command = new ExportToCsvCommand(viewModel, mockFileDialogService.Object, mockJsonToCsvConverter.Object);

        // Act
        await command.ExecuteAsync(null!);

        // Assert
        mockJsonToCsvConverter.Verify(c => c.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
