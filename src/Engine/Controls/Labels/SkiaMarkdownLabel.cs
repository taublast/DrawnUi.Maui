using DrawnUi.Maui.Draw;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Newtonsoft.Json.Linq;
using System.Windows.Input;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Will internally create spans from markdown.
/// Spans property must not be set directly.
/// </summary>
public class SkiaMarkdownLabel : SkiaLabel
{
    public SkiaMarkdownLabel()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .Build();
    }

    //properties defaults
    public static Color ColorLink = Colors.CornflowerBlue;
    public static Color ColorCodeBackground = Colors.DimGray;
    public static Color ColorCodeBlock = Color.Parse("#222222");
    public static Color ColorCode = Colors.White;
    public static Color ColorStrikeout = Colors.Red;

    //customizable
    public static string PrefixBullet = "• ";
    public static string PrefixNumbered = "{0}. ";

    public override void InvalidateText()
    {
        var markdownDocument = Markdig.Markdown.Parse(Text, _pipeline);

        Spans.Clear();

        isBold = false;
        isItalic = false;
        isHeading1 = false;
        isHeading2 = false;
        isCodeBlock = false;
        hadParagraph = false;
        isStrikethrough = false;

        foreach (var block in markdownDocument)
        {
            RenderBlock(block);
        }

        base.InvalidateText();
    }

    protected virtual TextSpan SpanWithAttributes(TextSpan span)
    {
        span.IsBold = isBold || span.IsBold;
        span.IsItalic = isItalic || span.IsItalic;

        span.Strikeout = isStrikethrough || span.Strikeout;
        if (span.Strikeout)
            span.StrikeoutColor = this.StrikeoutColor;

        if (isHeading1)
        {
            span.IsBold = true;
            span.FontSize += 6;
        }

        if (isHeading2)
        {
            span.IsBold = true;
            span.FontSize += 3;
        }
        return span;
    }


    #region PARSER

    protected bool hadParagraph;
    protected bool isBold;
    protected bool isItalic;
    protected bool isCodeBlock;
    protected bool isHeading1;
    protected bool isHeading2;
    protected bool isStrikethrough;
    protected readonly MarkdownPipeline _pipeline;

    protected void RenderBlock(Block block, Inline prefix = null)
    {
        var wasHeading1 = isHeading1;
        var wasHeading2 = isHeading2;
        var wasCode = isCodeBlock;

        if (block is HeadingBlock heading)
        {
            var level = heading.Level;
            isHeading1 = level == 1;
            if (!isHeading1)
            {
                isHeading2 = true;
            }

            if (heading.Inline != null)
            {
                if (hadParagraph)
                {
                    //start new line
                    RenderInline(new LineBreakInline());
                }

                hadParagraph = true;

                var inline = heading.Inline.FirstChild;
                while (inline != null)
                {
                    RenderInline(inline);
                    inline = inline.NextSibling;
                }

            }
        }
        else
        if (block is ParagraphBlock paragraphBlock)
        {
            if (hadParagraph)
            {
                //start new line
                RenderInline(new LineBreakInline());
            }

            if (prefix != null)
            {
                RenderInline(prefix);
            }

            hadParagraph = true;

            var inline = paragraphBlock.Inline.FirstChild;
            while (inline != null)
            {
                RenderInline(inline);
                inline = inline.NextSibling;
            }

        }
        else if (block is FencedCodeBlock codeBlock)
        {
            if (hadParagraph)
            {
                //start new line
                RenderInline(new LineBreakInline());
            }

            if (prefix != null)
            {
                RenderInline(prefix);
            }

            hadParagraph = true;

            isCodeBlock = true;

            var codeType = codeBlock.Info; //todo can display header like "csharp" etc
            var lineNb = 0;
            foreach (var line in codeBlock.Lines)
            {
                if (lineNb > 0)
                {
                    RenderInline(new LineBreakInline());
                }
                AddTextSpan(line.ToStringSafe(), (span) =>
                {
                    span.TextColor = CodeTextColor;
                    span.ParagraphColor = ColorCodeBlock;
                });
                lineNb++;
            }
        }
        else
        if (block is ListBlock listBlock)
        {
            foreach (var child in listBlock)
            {
                if (child is ListItemBlock listItem)
                {
                    if (listBlock.IsOrdered)
                    {
                        RenderOrderedListItem(listItem, listItem.Order);
                    }
                    else
                    {
                        RenderBulletListItem(listItem);
                    }
                }
                else
                {
                    //todo what can it be?..
                }
            }
        }
        else
        if (block is ListItemBlock listItem)
        {
            foreach (var line in listItem)
            {
                RenderBlock(line);
            }
        }

        isCodeBlock = wasCode;
        isHeading1 = wasHeading1;
        isHeading2 = wasHeading2;
    }

    protected void RenderOrderedListItem(ListItemBlock listItem, int startNumber)
    {
        LiteralInline prefix;
        var firstLine = true;
        foreach (var line in listItem)
        {
            if (firstLine)
            {
                // Add the number before rendering the list item content
                prefix = new LiteralInline(string.Format(PrefixNumbered, startNumber));
                firstLine = false;
            }
            else
            {
                prefix = null;
            }
            RenderBlock(line, prefix);
        }
    }

    protected void RenderBulletListItem(ListItemBlock listItem)
    {
        LiteralInline prefix;
        var firstLine = true;
        foreach (var line in listItem)
        {
            if (firstLine)
            {
                // Add the number before rendering the list item content
                prefix = new LiteralInline(PrefixBullet);
                firstLine = false;
            }
            else
            {
                prefix = null;
            }
            RenderBlock(line, prefix);
        }
    }

    protected void RenderInline(Inline inline)
    {
        bool wasBold = isBold;
        bool wasItalic = isItalic;
        bool wasStrikethrough = isStrikethrough;


        switch (inline)
        {

        case LineBreakInline lineBreak:
        //just add new line to previous span
        var last = Spans.LastOrDefault();
        if (last == null)
        {
            Spans.Add(new()
            {
                Text = "\n"
            }); ;
        }
        else
        {
            last.Text += "\n";
        }
        break;

        case LiteralInline literal:
        //todo detect available font
        AddTextSpan(literal.Content.ToStringSafe());
        break;

        case CodeInline code:
        AddCodeSpan(code);
        break;

        case LinkInline link:
        AddLinkSpan(link);
        break;

        case EmphasisInline emphasis:
        if (emphasis.DelimiterCount == 3)
        {
            if (emphasis.DelimiterChar is '_' or '*')
            {
                isBold = true;
                isItalic = true;
            }
        }
        else
        if (emphasis.DelimiterCount == 2)
        {
            if (emphasis.DelimiterChar == '~')
            {
                isStrikethrough = true;
            }
            else
            if (emphasis.DelimiterChar is '_' or '*')
            {
                isBold = true;
            }
        }
        else
        if (emphasis.DelimiterCount == 1)
        {
            if (emphasis.DelimiterChar is '_' or '*')
            {
                isItalic = true;
            }
        }

        var child = emphasis.FirstChild;
        while (child != null)
        {
            RenderInline(child);
            child = child.NextSibling;
        }

        break;

        }

        // Restore the previous state
        isBold = wasBold;
        isItalic = wasItalic;
        isStrikethrough = wasStrikethrough;
    }



    #endregion

    protected override void OnFontUpdated()
    {
        base.OnFontUpdated();

        OnTextChanged(this.Text);
    }

    public virtual (SKTypeface, int) FindBestTypefaceForString(string text)
    {
        var typefaceCoverage = new Dictionary<SKTypeface, HashSet<int>>();
        int symbol = 0;

        foreach (char c in text)
        {
            int codePoint = char.ConvertToUtf32(text, char.IsHighSurrogate(c) ? text.IndexOf(c) : 0);
            var typeface = SkiaFontManager.Manager.MatchCharacter(codePoint);

            if (typeface != null)
            {
                symbol = codePoint;

                if (!typefaceCoverage.ContainsKey(typeface))
                {
                    typefaceCoverage[typeface] = new HashSet<int>();
                }

                typefaceCoverage[typeface].Add(codePoint);
            }
        }

        // Now find the typeface with the most coverage
        var bestTypeface = typefaceCoverage.OrderByDescending(kvp => kvp.Value.Count).FirstOrDefault().Key;

        return (bestTypeface, symbol);
    }

    protected virtual void AddTextSpan(string text, Action<TextSpan> modifySpan = null)
    {
        if (TypeFace == null) //might happen early in cycle when set via Styles
            return;

        //var text = literal.Content.ToStringSafe();
        var currentIndex = 0;
        var spanStart = 0;
        var spanData = new List<(string Text, SKTypeface Typeface, int Symbol, bool shape)>();
        SKTypeface originalTypeFace = TypeFace;
        SKTypeface currentTypeFace = originalTypeFace;
        bool needShape = false;

        // First pass: Create raw span data
        while (currentIndex < text.Length)
        {
            int codePoint = char.ConvertToUtf32(text, currentIndex);
            string glyphText = char.ConvertFromUtf32(codePoint);
            bool isStandardSymbol = standardSymbols.Contains(glyphText[0]);

            void BreakSpanAndSwitchTypeface(SKTypeface newTypeface)
            {

                // When we switch typefaces, we add the span up to this point and then change the typeface
                var add = text.Substring(spanStart, currentIndex - spanStart);
                if (!needShape)
                {
                    needShape = SkiaLabel.UnicodeNeedsShaping(codePoint);
                }
                spanData.Add((add, currentTypeFace, codePoint, needShape));
                currentTypeFace = newTypeface;
                spanStart = currentIndex;
                needShape = false;
            }

            var glyph = SkiaLabel.GetGlyphs(glyphText, currentTypeFace).First();

            //switch to fallback font
            if (!isStandardSymbol && !glyph.IsAvailable)
            {
                SKTypeface newTypeFace = SkiaFontManager.Manager.MatchCharacter(codePoint);
                if (newTypeFace != null && newTypeFace != currentTypeFace)
                {
                    BreakSpanAndSwitchTypeface(newTypeFace);
                }
            }
            //maybe switch back to original font if possible
            else if (!isStandardSymbol && currentTypeFace != originalTypeFace)
            {
                var glyphInOriginal = SkiaLabel.GetGlyphs(glyphText, originalTypeFace).First();
                if (glyphInOriginal.IsAvailable)
                {
                    if (currentIndex - spanStart > 1)
                    {
                        //if we it's not the first symbol and we had some data to create a span with previous font..
                        BreakSpanAndSwitchTypeface(originalTypeFace);
                    }
                    else
                    {
                        //otherwise just switch font back to original
                        currentIndex--; //go back in time
                        currentTypeFace = originalTypeFace;
                        spanStart = currentIndex;
                        needShape = false;
                    }
                }
            }

            var advance = 1;

            if (char.IsSurrogatePair(text, currentIndex))
            {
                var sequence = EmojiData.IsEmojiModifierSequence(text, currentIndex);
                if (sequence > 0)
                {
                    needShape = true;
                    advance = sequence;
                }
                else
                    advance = 2;
            }

            currentIndex += advance;

            // When we reach the end, we add the remaining text as a span
            if (currentIndex >= text.Length)
            {
                BreakSpanAndSwitchTypeface(originalTypeFace);
            }
        }

        // Optimize spans data
        ProcessSpanData(ref spanData, originalTypeFace);

        // Create TextSpans from optimized spans data
        foreach (var (Text, Typeface, Symbol, Shape) in spanData)
        {
            var span = new TextSpan
            {
                Text = Text,
                TypeFace = Typeface,
                FontDetectedWith = Symbol,
                NeedShape = Shape
            };
            modifySpan?.Invoke(span);
            Spans.Add(SpanWithAttributes(span));
        }
    }

    protected static HashSet<char> standardSymbols = new HashSet<char> { ' ', '\n', '\r', '\t' };

    /// <summary>
    /// Do not let spans with non-default typeface end with standart symbols like ' ', move them to span with original typecase
    /// </summary>
    /// <param name="spanData"></param>
    /// <param name="originalTypeFace"></param>
    protected virtual void ProcessSpanData(ref List<(string Text, SKTypeface Typeface, int Symbol, bool Shape)> spanData, SKTypeface originalTypeFace)
    {
        for (int i = 0; i < spanData.Count - 1; i++)
        {
            var currentSpan = spanData[i];
            var nextSpan = spanData[i + 1];

            if (currentSpan.Typeface != originalTypeFace && standardSymbols.Contains(currentSpan.Text.Last()))
            {
                int standardSymbolIndex = currentSpan.Text.Length - 1;
                while (standardSymbolIndex >= 0 && standardSymbols.Contains(currentSpan.Text[standardSymbolIndex]))
                {
                    standardSymbolIndex--;
                }
                standardSymbolIndex++; // Adjust to point to the first standard symbol

                if (standardSymbolIndex > 0)
                {
                    spanData[i] = (currentSpan.Text.Substring(0, standardSymbolIndex), currentSpan.Typeface, currentSpan.Symbol, currentSpan.Shape);
                }
                else
                {
                    spanData.RemoveAt(i);
                    i--; // Adjust index to account for removed item
                    continue;
                }

                if (nextSpan.Typeface == originalTypeFace)
                {
                    spanData[i + 1] = (currentSpan.Text.Substring(standardSymbolIndex) + nextSpan.Text, nextSpan.Typeface, nextSpan.Symbol, nextSpan.Shape);
                }
                else
                {
                    // If the next span is not using the original typeface, insert a new span with the original typeface
                    spanData.Insert(i + 1, (currentSpan.Text.Substring(standardSymbolIndex), originalTypeFace, 0, false));
                }
            }
        }

    }

    #region GESTURES

    /// <summary>
    /// Url will be inside Tag
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public override ISkiaGestureListener OnSpanTapped(TextSpan span)
    {
        OnLinkTapped(span.Tag, span.Text);
        return base.OnSpanTapped(span);
    }

    #endregion

    #region CODE


    protected virtual void AddCodeSpan(CodeInline code)
    {
        string text = code.Content;

        AddTextSpan(text, (span) =>
        {
            span.TextColor = this.CodeTextColor;
            span.BackgroundColor = CodeBackgroundColor;
        });

    }

    #endregion

    #region LINKS


    protected virtual void AddLinkSpan(LinkInline link)
    {
        string text = GetLinkLabelText(link);
        if (string.IsNullOrEmpty(text))
            text = link.Url;

        AddTextSpan(text, (span) =>
        {
            Tag = link.Url;
            span.TextColor = this.LinkColor;
            span.Underline = true;
            span.ForceCaptureInput = true;
        });

    }

    public static readonly BindableProperty CommandLinkTappedProperty = BindableProperty.Create(nameof(CommandLinkTapped), typeof(ICommand),
        typeof(SkiaMarkdownLabel),
        null);
    public ICommand CommandLinkTapped
    {
        get { return (ICommand)GetValue(CommandLinkTappedProperty); }
        set { SetValue(CommandLinkTappedProperty, value); }
    }

    public event EventHandler<string> LinkTapped;

    public virtual void OnLinkTapped(string url, string text)
    {
        LinkTapped?.Invoke(this, url);
        CommandLinkTapped?.Execute(url);
    }

    protected virtual string GetLinkLabelText(LinkInline link)
    {
        var child = link.FirstChild;
        string labelText = "";
        while (child != null)
        {
            if (child is LiteralInline literal)
            {
                labelText += literal.Content;
            }
            child = child.NextSibling;
        }
        return labelText;
    }




    #endregion

    public static readonly BindableProperty StrikeoutColorProperty = BindableProperty.Create(
        nameof(StrikeoutColor),
        typeof(Color),
        typeof(SkiaLabel),
        ColorStrikeout,
        propertyChanged: NeedUpdateFont);

    public Color StrikeoutColor
    {
        get { return (Color)GetValue(StrikeoutColorProperty); }
        set { SetValue(StrikeoutColorProperty, value); }
    }

    public static readonly BindableProperty LinkColorProperty = BindableProperty.Create(
        nameof(LinkColor),
        typeof(Color),
        typeof(SkiaLabel),
        ColorLink,
        propertyChanged: NeedUpdateFont);

    public Color LinkColor
    {
        get { return (Color)GetValue(LinkColorProperty); }
        set { SetValue(LinkColorProperty, value); }
    }

    public static readonly BindableProperty CodeTextColorProperty = BindableProperty.Create(
        nameof(CodeTextColor),
        typeof(Color),
        typeof(SkiaLabel),
        ColorCode,
        propertyChanged: NeedUpdateFont);

    public Color CodeTextColor
    {
        get { return (Color)GetValue(CodeTextColorProperty); }
        set { SetValue(CodeTextColorProperty, value); }
    }

    public static readonly BindableProperty CodeBlockBackgroundColorProperty = BindableProperty.Create(
        nameof(CodeBlockBackgroundColor),
        typeof(Color),
        typeof(SkiaLabel),
        ColorCodeBlock,
        propertyChanged: NeedUpdateFont);

    public Color CodeBlockBackgroundColor
    {
        get { return (Color)GetValue(CodeBlockBackgroundColorProperty); }
        set { SetValue(CodeBlockBackgroundColorProperty, value); }
    }

    public static readonly BindableProperty CodeBackgroundColorProperty = BindableProperty.Create(
        nameof(CodeBackgroundColor),
        typeof(Color),
        typeof(SkiaLabel),
        ColorCodeBackground,
        propertyChanged: NeedUpdateFont);

    public Color CodeBackgroundColor
    {
        get { return (Color)GetValue(CodeBackgroundColorProperty); }
        set { SetValue(CodeBackgroundColorProperty, value); }
    }

}