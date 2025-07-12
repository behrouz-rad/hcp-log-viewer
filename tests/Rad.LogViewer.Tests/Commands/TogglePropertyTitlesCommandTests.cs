// © 2025 Behrouz Rad. All rights reserved.

using System.Reactive.Linq;
using Rad.LogViewer.App.Commands.Implementations;
using Rad.LogViewer.App.Constants;
using Rad.LogViewer.App.Services.Settings;
using Rad.LogViewer.App.ViewModels;

namespace Rad.LogViewer.Tests.Commands;

public class TogglePropertyTitlesCommandTests
{
    private readonly Mock<ISettingsService> _mockSettingsService;
    private readonly MainViewModel _viewModel;

    public TogglePropertyTitlesCommandTests()
    {
        _mockSettingsService = new Mock<ISettingsService>();

        var mockLogFileParser = new Mock<App.Services.Parsers.ILogFileParser>();
        var mockFileDialogService = new Mock<App.Services.Dialogs.IFileDialogService>();
        var mockJsonToCsvConverter = new Mock<App.Services.Converters.IJsonToCsvConverter>();
        var mockThemeService = new Mock<App.Services.Theme.IThemeService>();

        _mockSettingsService
            .Setup(s => s.GetSetting(AppConstants.Settings.ShowPropertyTitles, It.IsAny<bool>()))
            .Returns(true);

        _viewModel = new MainViewModel(
            mockLogFileParser.Object,
            mockFileDialogService.Object,
            mockJsonToCsvConverter.Object,
            mockThemeService.Object,
            _mockSettingsService.Object);
    }

    [Fact]
    public async Task Execute_ShouldToggleShowPropertyTitles()
    {
        // Arrange
        var initialValue = _viewModel.ShowPropertyTitles;
        var command = new TogglePropertyTitlesCommand(_viewModel, _mockSettingsService.Object);

        _mockSettingsService
            .Setup(s => s.SaveSettingAsync(AppConstants.Settings.ShowPropertyTitles, !initialValue))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await command.Command.Execute().FirstAsync();

        // Assert
        _viewModel.ShowPropertyTitles.Should().Be(!initialValue);
        _mockSettingsService.Verify();
    }
}
