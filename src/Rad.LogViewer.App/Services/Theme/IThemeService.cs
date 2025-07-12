// © 2025 Behrouz Rad. All rights reserved.

namespace Rad.LogViewer.App.Services.Theme;

public interface IThemeService
{
    public bool IsDarkTheme { get; }
    public void ToggleTheme();
    public void ApplyTheme();
    public Task SaveThemePreferenceAsync();
}
