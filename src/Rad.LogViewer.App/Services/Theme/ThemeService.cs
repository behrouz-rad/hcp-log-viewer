// © 2025 Behrouz Rad. All rights reserved.

using Avalonia;
using Avalonia.Styling;
using Rad.LogViewer.App.Constants;
using Rad.LogViewer.App.Services.Settings;

namespace Rad.LogViewer.App.Services.Theme;

public class ThemeService : IThemeService
{
    private readonly ISettingsService _settingsService;
    private bool _isDarkTheme;

    public bool IsDarkTheme => _isDarkTheme;

    public ThemeService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        LoadThemePreference();
    }

    public void ToggleTheme()
    {
        _isDarkTheme = !_isDarkTheme;
        ApplyTheme();
        _ = SaveThemePreferenceAsync();
    }

    public void ApplyTheme()
    {
        Application.Current!.RequestedThemeVariant = _isDarkTheme
            ? ThemeVariant.Dark
            : ThemeVariant.Light;
    }

    public async Task SaveThemePreferenceAsync()
    {
        await _settingsService.SaveSettingAsync(AppConstants.Settings.IsDarkTheme, _isDarkTheme);
    }

    private void LoadThemePreference()
    {
        _isDarkTheme = _settingsService.GetSetting(AppConstants.Settings.IsDarkTheme, false);
    }
}
