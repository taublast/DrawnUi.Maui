using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Draw;

namespace DrawnUi.Maui.Draw;

public interface IDrawnBase : IDisposable, ICanBeUpdatedWithContext
{
    SKRect DrawingRect { get; }

    string Tag { get; }

    bool IsVisible { get; set; }

    bool IsDisposed { get; }

    bool IsVisibleInViewTree();

    /// <summary>
    /// Obtain rectangle visible on the screen to avoid offscreen rendering etc
    /// </summary>
    /// <returns></returns>
    public ScaledRect GetOnScreenVisibleArea();

    /// <summary>
    /// Invalidates the measured size. May or may not call Update() inside, depends on control
    /// </summary>
    void Invalidate();

    /// <summary>
    /// If need the re-measure all parents because child-auto-size has changed
    /// </summary>
    void InvalidateParents();

    /// <summary>
    /// Creates a new disposable SKPath for clipping content according to the control shape and size.
    /// Create this control clip for painting content.
    /// Pass arguments if you want to use some time-frozen data for painting at any time from any thread..
    /// If applyPosition is false will create clip without using drawing posiition, like if was drawing at 0,0.
    /// </summary>
    /// <returns></returns>
    SKPath CreateClip(object arguments, bool usePosition);

    bool RegisterAnimator(ISkiaAnimator animator);

    void UnregisterAnimator(Guid uid);

    IEnumerable<ISkiaAnimator> UnregisterAllAnimatorsByType(Type type);

    public void RegisterGestureListener(ISkiaGestureListener gestureListener);

    public void UnregisterGestureListener(ISkiaGestureListener gestureListener);

    /// <summary>
    /// Executed after the rendering
    /// </summary>
    public List<IOverlayEffect> PostAnimators { get; }


    /// <summary>
    /// For code-behind access of children, XAML is using Children property
    /// </summary>
    List<SkiaControl> Views { get; }

    /// <summary>
    /// Directly adds a view to the control, without any layouting. Use this instead of Views.Add() to avoid memory leaks etc
    /// </summary>
    /// <param name="view"></param>
    public void AddSubView(SkiaControl view);

    /// <summary>
    /// Directly removes a view from the control, without any layouting.
    /// Use this instead of Views.Remove() to avoid memory leaks etc
    /// </summary>
    /// <param name="view"></param>
    public void RemoveSubView(SkiaControl view);

    public ScaledSize MeasuredSize { get; }

    SKRect Destination { get; }

    public double HeightRequest { get; set; }

    public double WidthRequest { get; set; }

    public double Height { get; }

    public double Width { get; }

    public double TranslationX { get; }

    public double TranslationY { get; }

    public bool InputTransparent { get; set; }

    bool IsClippedToBounds { get; set; }

    bool ClipEffects { get; set; }

    bool UpdateLocked { get; }

    /// <summary>
    /// This is needed by layout to track which child changed to sometimes avoid recalculating other children
    /// </summary>
    /// <param name="skiaControl"></param>
    void InvalidateByChild(SkiaControl skiaControl);

    public bool ShouldInvalidateByChildren { get; }

    float RenderingScale { get; }

    double X { get; }

    double Y { get; }

    void InvalidateViewport();

    void Repaint();

    void InvalidateViewsList();

}