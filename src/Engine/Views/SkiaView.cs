namespace DrawnUi.Maui.Views;

public partial class SkiaView : SKCanvasView, ISkiaDrawable
{
    public bool IsHardwareAccelerated => false;

    public SKSurface CreateStandaloneSurface(int width, int height)
    {
        return SKSurface.Create(new SKImageInfo(width, height));
    }

    public Func<SKCanvas, SKRect, bool> OnDraw { get; set; }

    private DrawnView _parent;

    public void Dispose()
    {
        _surface = null;
        PaintSurface -= OnPaintingSurface;
        _parent = null;
    }

    public SkiaView(DrawnView parent)
    {
        _parent = parent;
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

            _parent?.DisconnectedHandler();
        }
        else
        {
            PaintSurface -= OnPaintingSurface;
            PaintSurface += OnPaintingSurface;

            _parent?.ConnectedHandler();
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

    public bool IsDrawing { get; protected set; }

    private void OnPaintingSurface(object sender, SKPaintSurfaceEventArgs paintArgs)
    {
        //IsDrawing = true;

        //todo calculate fps for this code:
        _fps = 1.0 / (DateTime.Now - _lastFrame).TotalSeconds;
        _lastFrame = DateTime.Now;

        if (OnDraw != null)
        {
            _surface = paintArgs.Surface;
            bool invalidate = OnDraw.Invoke(paintArgs.Surface.Canvas, new SKRect(0, 0, paintArgs.Info.Width, paintArgs.Info.Height));

#if ANDROID
            if (invalidate && _fps < 120)
            {
                InvalidateSurface();
            }
            else
#endif
            IsDrawing = false;
        }

    }
}
