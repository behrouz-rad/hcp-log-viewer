using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using System.Text.RegularExpressions;

namespace Hcp.LogViewer.App.Controls;

internal sealed class HighlightedTextBlock : TextBlock
{
    public static readonly StyledProperty<string> SearchTermProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, string>(nameof(SearchTerm));

    public static readonly StyledProperty<IBrush> HighlightBrushProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, IBrush>(nameof(HighlightBrush),
            new SolidColorBrush(Color.Parse("#FFFFB266")));

    public static readonly StyledProperty<bool> IsCaseSensitiveProperty =
        AvaloniaProperty.Register<HighlightedTextBlock, bool>(nameof(IsCaseSensitive), false);

    public string SearchTerm
    {
        get => GetValue(SearchTermProperty);
        set => SetValue(SearchTermProperty, value);
    }

    public IBrush HighlightBrush
    {
        get => GetValue(HighlightBrushProperty);
        set => SetValue(HighlightBrushProperty, value);
    }

    public bool IsCaseSensitive
    {
        get => GetValue(IsCaseSensitiveProperty);
        set => SetValue(IsCaseSensitiveProperty, value);
    }

    static HighlightedTextBlock()
    {
        TextProperty.Changed.AddClassHandler<HighlightedTextBlock>((x, e) => x.UpdateText());
        SearchTermProperty.Changed.AddClassHandler<HighlightedTextBlock>((x, e) => x.UpdateText());
        IsCaseSensitiveProperty.Changed.AddClassHandler<HighlightedTextBlock>((x, e) => x.UpdateText());
    }

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

    private void ClearInlines()
    {
        Inlines?.Clear();
    }
}
