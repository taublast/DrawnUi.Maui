namespace DrawnUi.Draw;

public struct LineGlyph
{
    public static LineGlyph FromGlyph(UsedGlyph glyph, float position, float width)
    {
        return new LineGlyph()
        {
            Id = glyph.Id,
            Text = glyph.Text,
            Position = position,
            Width = width,
            Symbol = glyph.Symbol,
            IsAvailable = glyph.IsAvailable
        };
    }

    public ushort Id { get; set; }

    public int Symbol { get; set; }

    public float Position { get; set; }

    /// <summary>
    /// Measured text with advance
    /// </summary>
    public float Width { get; set; }

    public static LineGlyph Move(LineGlyph existing, float position)
    {
        return existing with { Position = position };
    }

    public string Text { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsNumber()
    {
        // This will only work correctly if Text is a single character.
        return Text.Length == 1 && char.IsDigit(Text[0]);
    }
}