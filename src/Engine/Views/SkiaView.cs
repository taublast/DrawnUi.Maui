namespace DrawnUi.Maui.Views;



public partial class SkiaView : SKCanvasView, ISkiaDrawable
{
    public bool IsHardwareAccelerated => false;

    public SKSurface CreateStandaloneSurface(int width, int height)
    {
        return SKSurface.Create(new SKImageInfo(width, height));
    }

    public Func<SKCanvas, SKRect, bool> OnDraw { get; set; }

    public DrawnView Superview { get; protected set; }

    public void Dispose()
    {
        _surface = null;
        PaintSurface -= OnPaintingSurface;
        Superview = null;
    }

    public SkiaView(DrawnView superview)
    {
        Superview = superview;
        EnableTouchEvents = false;
    }

    public void Disconnect()
    {
        PaintSurface -= OnPaintingSurface;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler == null)
        {
            PaintSurface -= OnPaintingSurface;

            Superview?.DisconnectedHandler();
        }
        else
        {
            PaintSurface -= OnPaintingSurface;
            PaintSurface += OnPaintingSurface;

            Superview?.ConnectedHandler();
        }
    }

    SKSurface _surface;
    private DateTime _lastFrame;
    private double _fps;

    public SKSurface Surface
    {
        get
        {
            return _surface;
        }
    }

    public double FPS
    {
        get
        {
            return _fps;
        }
    }

    public double FrameTime { get; protected set; }

    public bool IsDrawing { get; protected set; }

    private void OnPaintingSurface(object sender, SKPaintSurfaceEventArgs paintArgs)
    {
        IsDrawing = true;

        _fps = 1.0 / (DateTime.Now - _lastFrame).TotalSeconds;
        _lastFrame = DateTime.Now;

        FrameTime = Super.GetCurrentTimeNanos();
        
        if (OnDraw != null && Super.EnableRendering)
        {
            _surface = paintArgs.Surface;
            bool invalidate = OnDraw.Invoke(paintArgs.Surface.Canvas, new SKRect(0, 0, paintArgs.Info.Width, paintArgs.Info.Height));
            if (invalidate && Super.EnableRendering) //if we didnt call update because IsDrawing was true need to kick here
            {
                IsDrawing = false;
#if ANDROID
                if (_fps<120)
                    InvalidateSurface();
                else
#else
                Superview.Update();
#endif
                return;
            }
        }

        IsDrawing = false;

    }

}
