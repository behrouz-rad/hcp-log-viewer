// © 2025 Behrouz Rad. All rights reserved.

using Hcp.LogViewer.App.Helpers;

namespace Hcp.LogViewer.Tests.Helpers;

public class WhitespaceRegExTests
{
    [Fact]
    public void EveryWhitespace_ShouldMatchSpaces()
    {
        // Arrange
        const string input = "Hello World";

        // Act
        var result = WhitespaceRegEx.EveryWhitespace().Replace(input, "_");

        // Assert
        result.Should().Be("Hello_World");
    }

    [Fact]
    public void EveryWhitespace_ShouldMatchMultipleWhitespaceTypes()
    {
        // Arrange
        const string input = "Hello\t\r\n World";

        // Act
        var result = WhitespaceRegEx.EveryWhitespace().Replace(input, "_");

        // Assert
        result.Should().Be("Hello_World");
    }

    [Fact]
    public void EveryWhitespace_ShouldMatchConsecutiveWhitespace()
    {
        // Arrange
        const string input = "Hello    World";

        // Act
        var result = WhitespaceRegEx.EveryWhitespace().Replace(input, "_");

        // Assert
        result.Should().Be("Hello_World");
    }

    [Fact]
    public void EveryWhitespace_ShouldNotMatchNonWhitespace()
    {
        // Arrange
        const string input = "HelloWorld";

        // Act
        var result = WhitespaceRegEx.EveryWhitespace().Replace(input, "_");

        // Assert
        result.Should().Be("HelloWorld");
    }

    [Fact]
    public void EveryWhitespace_ShouldHandleEmptyString()
    {
        // Arrange
        const string input = "";

        // Act
        var result = WhitespaceRegEx.EveryWhitespace().Replace(input, "_");

        // Assert
        result.Should().Be("");
    }

    [Fact]
    public void EveryWhitespace_ShouldHandleOnlyWhitespace()
    {
        // Arrange
        const string input = "   \t\r\n   ";

        // Act
        var result = WhitespaceRegEx.EveryWhitespace().Replace(input, "_");

        // Assert
        result.Should().Be("_");
    }
}
