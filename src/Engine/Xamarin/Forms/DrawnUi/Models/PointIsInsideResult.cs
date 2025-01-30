using DrawnUi.Maui.Extensions;

namespace DrawnUi.Maui.Draw;

public class PointIsInsideResult
{
    public bool IsInside
    {
        get
        {
            return Area.ContainsInclusive(Point);
        }
    }
    public SKPoint Point { get; set; }
    public SKRect Area { get; set; }
}
