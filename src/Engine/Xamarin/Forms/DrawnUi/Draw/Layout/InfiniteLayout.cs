namespace DrawnUi.Maui.Draw;


public class InfiniteLayout : ContentLayout
{
    protected override SKRect GetContentAvailableRect(SKRect destination)
    {
        var childRect = new SKRect(destination.Left, destination.Top, destination.Right, destination.Bottom);
        if (Orientation == ScrollOrientation.Both)
        {
            childRect.Right = float.PositiveInfinity;
            childRect.Bottom = float.PositiveInfinity;
        }
        else
        if (Orientation == ScrollOrientation.Vertical)
        {
            childRect.Right = destination.Right;
            childRect.Bottom = float.PositiveInfinity;
        }
        if (Orientation == ScrollOrientation.Horizontal)
        {
            childRect.Right = float.PositiveInfinity;
            childRect.Bottom = destination.Bottom;
        }
        return childRect;
    }
}