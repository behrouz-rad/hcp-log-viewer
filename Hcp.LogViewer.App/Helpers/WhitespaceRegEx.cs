using System.Text.RegularExpressions;

namespace Hcp.LogViewer.App.Helpers;

internal static partial class WhitespaceRegEx
{
    [GeneratedRegex(@"\s+", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    public static partial Regex EveryWhitespace();
}
