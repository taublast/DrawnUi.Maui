namespace DrawnUi.Draw;

public struct UsedGlyph
{
    public ushort Id { get; set; }

    public string Text { get; set; }

    public int Symbol { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsNumber()
    {
        // This will only work correctly if Text is a single character.
        return Text.Length == 1 && char.IsDigit(Text[0]);
    }
}