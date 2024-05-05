namespace DrawnUi.Maui.Draw;

public class SkiaGesturesParameters
{
    //public SKPoint PanningOffset { get; set; }
    public TouchActionResult Type { get; set; }
    public TouchActionEventArgs Event { get; set; }

    public static SkiaGesturesParameters Create(TouchActionResult action, TouchActionEventArgs args)
    {
        return new SkiaGesturesParameters()
        {
            Type = action,
            Event = args
        };
    }

    public static SkiaGesturesParameters Empty
    {
        get
        {
            return new SkiaGesturesParameters();
        }
    }
}

public struct GestureEventProcessingInfo
{
    public SKPoint childOffset { get; set; }

    public SKPoint childOffsetDirect { get; set; }

    public ISkiaGestureListener alreadyConsumed { get; set; }

    public GestureEventProcessingInfo(SKPoint childOffset1, SKPoint childOffsetDirect1, ISkiaGestureListener wasConsumed1)
    {
        childOffset = childOffset1;
        childOffsetDirect = childOffsetDirect1;
        wasConsumed1 = wasConsumed1;
    }

    public GestureEventProcessingInfo()
    {

    }

    public static GestureEventProcessingInfo Empty
    {
        get
        {
            return new GestureEventProcessingInfo();
        }
    }
}