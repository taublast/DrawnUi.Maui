namespace DrawnUi.Maui.Draw;

public interface ILayoutInsideViewport : IInsideViewport
{


    /// <summary>
    /// The point here is the rendering location, always on screen
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public ContainsPointResult GetVisibleChildIndexAt(SKPoint point);

    /// <summary>
    /// The point here is the position inside parent, can be offscreen
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public ContainsPointResult GetChildIndexAt(SKPoint point);
}