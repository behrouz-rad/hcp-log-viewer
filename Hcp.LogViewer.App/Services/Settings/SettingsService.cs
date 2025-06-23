// © 2025 Behrouz Rad. All rights reserved.

using System.Reflection;
using System.Text.Json;
using Hcp.LogViewer.App.Constants;

namespace Hcp.LogViewer.App.Services.Settings;

/// <summary>
/// Implementation of the settings service
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private Dictionary<string, object> _settings;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SettingsService()
    {
        // Store settings next to the executable for cross-platform compatibility
        string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        _settingsFilePath = Path.Combine(executablePath, AppConstants.SettingsFileName);
        _settings = LoadSettings();
    }

    public T GetSetting<T>(string key, T? defaultValue = default)
    {
        if (_settings.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
            {
                var result = JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), _jsonOptions);
                return result != null ? result : defaultValue!;
            }

            return (T)value;
        }

        return defaultValue!;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        _settings[key] = value!;
        await SaveSettingsAsync();
    }

    private Dictionary<string, object> LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json, _jsonOptions)
                    ?? new Dictionary<string, object>();
            }
        }
        catch
        {
            // Silently fail if we can't read the settings file
        }

        return new Dictionary<string, object>();
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, _jsonOptions);
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
        catch
        {
            // Silently fail if we can't write the settings file
        }
    }
}
