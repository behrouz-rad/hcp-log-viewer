// © 2025 Behrouz Rad. All rights reserved.

using System.Threading.Tasks;

namespace Hcp.LogViewer.App.Services.Settings;

/// <summary>
/// Service for managing application settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets a setting value by key
    /// </summary>
    /// <typeparam name="T">Type of the setting value</typeparam>
    /// <param name="key">Setting key</param>
    /// <param name="defaultValue">Default value if setting doesn't exist</param>
    /// <returns>Setting value or default value</returns>
    T GetSetting<T>(string key, T? defaultValue = default);

    /// <summary>
    /// Sets a setting value
    /// </summary>
    /// <typeparam name="T">Type of the setting value</typeparam>
    /// <param name="key">Setting key</param>
    /// <param name="value">Setting value</param>
    /// <returns>Task representing the save operation</returns>
    Task SaveSettingAsync<T>(string key, T value);
}
