// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using Avalonia.Data.Converters;

namespace Hcp.LogViewer.App.Converters;

/// <summary>
/// Converts a string to a boolean indicating whether it is null or empty.
/// </summary>
/// <remarks>
/// This converter is used in XAML bindings to check if a string is null or empty.
/// </remarks>
internal sealed class StringConverters : IValueConverter
{
    public static readonly StringConverters Instance = new();

    /// <summary>
    /// Converts a string to a boolean indicating whether it is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">Culture information.</param>
    /// <returns>True if the string is null or empty; otherwise, false.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value as string);
    }

    /// <summary>
    /// Not supported for this converter.
    /// </summary>
    /// <exception cref="NotSupportedException">Always thrown as conversion back is not supported.</exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converts a string to a boolean indicating whether it is not null or empty.
/// </summary>
/// <remarks>
/// This converter is used in XAML bindings to check if a string has content.
/// </remarks>
internal sealed class StringIsNotNullOrEmptyConverter : IValueConverter
{
    /// <summary>
    /// Gets the singleton instance of the converter.
    /// </summary>
    public static readonly StringIsNotNullOrEmptyConverter Instance = new();

    /// <summary>
    /// Converts a string to a boolean indicating whether it is not null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">Culture information.</param>
    /// <returns>True if the string has content; otherwise, false.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    /// <summary>
    /// Not supported for this converter.
    /// </summary>
    /// <exception cref="NotSupportedException">Always thrown as conversion back is not supported.</exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
