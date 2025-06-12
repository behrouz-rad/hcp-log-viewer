// © 2025 Behrouz Rad. All rights reserved.

using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace Hcp.LogViewer.App.Controls;

/// <summary>
/// A TextBlock control that highlights occurrences of a search term within its text.
/// </summary>
internal sealed class HighlightedTextBlock : TextBlock
{
    /// <summary>
    /// Defines the SearchTerm dependency property.
    /// </summary>
    public static readonly StyledProperty<string> SearchTermProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, string>(nameof(SearchTerm));

    /// <summary>
    /// Defines the HighlightBrush dependency property.
    /// </summary>
    public static readonly StyledProperty<IBrush> HighlightBrushProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, IBrush>(nameof(HighlightBrush),
            new SolidColorBrush(Color.Parse("#FFFFB266")));

    /// <summary>
    /// Defines the IsCaseSensitive dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCaseSensitiveProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, bool>(nameof(IsCaseSensitive), false);

    /// <summary>
    /// Gets or sets the search term to highlight in the text.
    /// </summary>
    public string SearchTerm
    {
        get => GetValue(SearchTermProperty);
        set => SetValue(SearchTermProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to highlight matching text.
    /// </summary>
    public IBrush HighlightBrush
    {
        get => GetValue(HighlightBrushProperty);
        set => SetValue(HighlightBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the search is case-sensitive.
    /// </summary>
    public bool IsCaseSensitive
    {
        get => GetValue(IsCaseSensitiveProperty);
        set => SetValue(IsCaseSensitiveProperty, value);
    }

    /// <summary>
    /// Static constructor to register property change handlers.
    /// </summary>
    static HighlightedTextBlock()
    {
        TextProperty.Changed.AddClassHandler<HighlightedTextBlock>((x, e) => x.UpdateText());
        SearchTermProperty.Changed.AddClassHandler<HighlightedTextBlock>((x, e) => x.UpdateText());
        IsCaseSensitiveProperty.Changed.AddClassHandler<HighlightedTextBlock>((x, e) => x.UpdateText());
    }

    /// <summary>
    /// Updates the text display with highlighted search terms.
    /// </summary>
    private void UpdateText()
    {
        if (string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(SearchTerm))
        {
            ClearInlines();
            return;
        }

        var text = Text;
        var searchTerm = SearchTerm;

        var options = IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
        var regex = new Regex(Regex.Escape(searchTerm), options, TimeSpan.FromSeconds(1));
        var matches = regex.Matches(text);

        if (matches.Count == 0)
        {
            ClearInlines();
            return;
        }

        var inlineCollection = new InlineCollection();
        int lastIndex = 0;

        foreach (Match match in matches)
        {
            // Add the text before the match
            if (match.Index > lastIndex)
            {
                inlineCollection.Add(new Run { Text = text.Substring(lastIndex, match.Index - lastIndex) });
            }

            // Add the highlighted match
            inlineCollection.Add(new Run
            {
                Text = text.Substring(match.Index, match.Length),
                Background = HighlightBrush,
                FontWeight = FontWeight.Bold
            });

            lastIndex = match.Index + match.Length;
        }

        // Add any remaining text after the last match
        if (lastIndex < text.Length)
        {
            inlineCollection.Add(new Run { Text = text.Substring(lastIndex) });
        }

        Inlines = inlineCollection;
    }

    /// <summary>
    /// Clears all inline elements from the text block.
    /// </summary>
    private void ClearInlines()
    {
        Inlines?.Clear();
    }
}
