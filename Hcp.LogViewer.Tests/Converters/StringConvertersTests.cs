// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using Hcp.LogViewer.App.Converters;

namespace Hcp.LogViewer.Tests.Converters;

public class StringConvertersTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("test", false)]
    public void StringIsNullOrEmptyConverter_Convert_ShouldReturnCorrectValue(string? input, bool expected)
    {
        // Arrange
        var converter = StringIsNullOrEmptyConverter.Instance;

        // Act
        var result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void StringIsNullOrEmptyConverter_ConvertBack_ShouldThrowNotSupportedException()
    {
        // Arrange
        var converter = StringIsNullOrEmptyConverter.Instance;

        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(string), null, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("test", true)]
    public void StringIsNotNullOrEmptyConverter_Convert_ShouldReturnCorrectValue(string? input, bool expected)
    {
        // Arrange
        var converter = StringIsNotNullOrEmptyConverter.Instance;

        // Act
        var result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void StringIsNotNullOrEmptyConverter_ConvertBack_ShouldThrowNotSupportedException()
    {
        // Arrange
        var converter = StringIsNotNullOrEmptyConverter.Instance;

        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(true, typeof(string), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void StringIsNullOrEmptyConverter_Convert_WithNonStringInput_ShouldHandleGracefully()
    {
        // Arrange
        var converter = StringIsNullOrEmptyConverter.Instance;
        const int input = 123;

        // Act
        var result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(true); // Non-string is treated as null
    }

    [Fact]
    public void StringIsNotNullOrEmptyConverter_Convert_WithNonStringInput_ShouldHandleGracefully()
    {
        // Arrange
        var converter = StringIsNotNullOrEmptyConverter.Instance;
        const int input = 123; // Non-string input

        // Act
        var result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(false); // Non-string is treated as null
    }
}
