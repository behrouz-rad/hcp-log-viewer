using System.Globalization;
using Avalonia.Data.Converters;

namespace Hcp.LogViewer.App.Converters;

internal sealed class ObjectIsNullConverter : IValueConverter
{
    public static readonly ObjectIsNullConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

internal sealed class ObjectIsNotNullConverter : IValueConverter
{
    public static readonly ObjectIsNotNullConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
