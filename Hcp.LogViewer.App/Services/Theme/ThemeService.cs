// © 2025 Behrouz Rad. All rights reserved.

using Avalonia;
using Avalonia.Styling;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hcp.LogViewer.App.Services.Theme;

public partial class ThemeService : IThemeService
{
    private const string SettingsFileName = "theme_settings.json";
    private readonly string _settingsFilePath;
    private bool _isDarkTheme;

    public bool IsDarkTheme => _isDarkTheme;

    public ThemeService()
    {
        // Store settings next to the executable for cross-platform compatibility
        string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        _settingsFilePath = Path.Combine(executablePath, SettingsFileName);
        
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
        try
        {
            var settings = new ThemeSettings { IsDarkTheme = _isDarkTheme };
            var json = JsonSerializer.Serialize(settings);
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
        catch
        {
            // Silently fail if we can't write the settings file
        }
    }

    private void LoadThemePreference()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<ThemeSettings>(json);
                _isDarkTheme = settings?.IsDarkTheme ?? false;
            }
            else
            {
                _isDarkTheme = false; // Default to light theme
            }
        }
        catch
        {
            _isDarkTheme = false; // Default to light theme on error
        }
    }
}
