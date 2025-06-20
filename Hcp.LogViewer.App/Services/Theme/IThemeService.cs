// © 2025 Behrouz Rad. All rights reserved.

namespace Hcp.LogViewer.App.Services.Theme;

public interface IThemeService
{
    bool IsDarkTheme { get; }
    void ToggleTheme();
    void ApplyTheme();
    Task SaveThemePreferenceAsync();
}
