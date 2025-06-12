// © 2025 Behrouz Rad. All rights reserved.

using System.Text.RegularExpressions;

namespace Hcp.LogViewer.App.Helpers;

/// <summary>
/// Provides generated regular expressions for whitespace matching.
/// </summary>
internal static partial class WhitespaceRegEx
{
    /// <summary>
    /// Returns a regex that matches one or more whitespace characters.
    /// </summary>
    /// <returns>A compiled regex that matches any whitespace sequence.</returns>
    [GeneratedRegex(@"\s+", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    public static partial Regex EveryWhitespace();
}
