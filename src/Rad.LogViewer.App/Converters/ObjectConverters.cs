// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using Avalonia.Data.Converters;

namespace Rad.LogViewer.App.Converters;

/// <summary>
/// Converts an object to a boolean indicating whether it is null.
/// </summary>
/// <remarks>
/// This converter is used in XAML bindings to check if an object is null.
/// </remarks>
internal sealed class ObjectIsNullConverter : IValueConverter
{
    public static readonly ObjectIsNullConverter Instance = new();

    /// <summary>
    /// Converts an object to a boolean indicating whether it is null.
    /// </summary>
    /// <param name="value">The object to check.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">Culture information.</param>
    /// <returns>True if the value is null; otherwise, false.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null;
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
/// Converts an object to a boolean indicating whether it is not null.
/// </summary>
/// <remarks>
/// This converter is used in XAML bindings to check if an object is not null.
/// </remarks>
internal sealed class ObjectIsNotNullConverter : IValueConverter
{
    public static readonly ObjectIsNotNullConverter Instance = new();

    /// <summary>
    /// Converts an object to a boolean indicating whether it is not null.
    /// </summary>
    /// <param name="value">The object to check.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">Culture information.</param>
    /// <returns>True if the value is not null; otherwise, false.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
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
