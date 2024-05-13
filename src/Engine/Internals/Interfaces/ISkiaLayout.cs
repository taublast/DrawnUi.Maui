namespace DrawnUi.Maui.Draw;

public interface ISkiaLayout : ISkiaControl, ILayoutInsideViewport //, IList<SkiaControl>
{
    bool NeedAutoSize { get; }

    bool NeedAutoHeight { get; }

    bool NeedAutoWidth { get; }

}