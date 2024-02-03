namespace DrawnUi.Maui.Draw;

public struct RangeF
{
    public float Start { get; set; }
    public float End { get; set; }

    public RangeF(float start, float end)
    {
        Start = start;
        End = end;
    }

    public float Length => Math.Abs(End - Start);

    public float Delta => End - Start;
}

public class ScaledSize
{
    public ScaledSize()
    {
        Scale = 1;
        Units = SKSize.Empty;
        Pixels = SKSize.Empty;
    }
    public float Scale { get; set; }
    public SKSize Units { get; set; }
    public SKSize Pixels { get; set; }

    public ScaledSize Clone()
    {
        return new()
        {
            Scale = this.Scale,
            Units = this.Units,
            Pixels = this.Pixels
        };
    }

    public static ScaledSize CreateEmpty(float scale)
    {
        return new()
        {
            Scale = scale,
            Units = SKSize.Empty,
            Pixels = SKSize.Empty
        };
    }

    public static ScaledSize Empty
    {
        get
        {
            return new()
            {
                Scale = 1,
                Units = SKSize.Empty,
                Pixels = SKSize.Empty
            };
        }
    }

    public static ScaledSize FromUnits(float width, float height, float scale)
    {
        if (double.IsNaN(width))
            width = -1;
        if (double.IsNaN(height))
            height = -1;

        var nWidth = (float)Math.Round(width * scale);
        if (nWidth < 0)
            nWidth = -1;
        if (float.IsInfinity(width))
        {
            nWidth = float.PositiveInfinity;
        }

        var nHeight = (float)Math.Round(height * scale);
        if (nHeight < 0)
            nHeight = -1;

        if (float.IsInfinity(height))
        {
            nHeight = float.PositiveInfinity;
        }

        return new ScaledSize()
        {
            Scale = scale,
            Units = new SKSize(width, height),
            Pixels = new SKSize(nWidth, nHeight)
        };
    }


    public static ScaledSize FromPixels(SKSize size, float scale)
    {
        return FromPixels((float)Math.Round(size.Width), (float)Math.Round(size.Height), scale);
    }

    public static ScaledSize FromPixels(float width, float height, float scale)
    {
        if (double.IsNaN(width))
            width = -1;
        if (double.IsNaN(height))
            height = -1;

        var nWidth = width / scale;
        if (nWidth < 0)
            nWidth = -1;

        var nHeight = height / scale;
        if (nHeight < 0)
            nHeight = -1;

        return new ScaledSize()
        {
            Scale = scale,
            Pixels = new SKSize(width, height),
            Units = new SKSize(nWidth, nHeight)
        };
    }
}