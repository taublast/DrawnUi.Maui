namespace DrawnUi.Maui.Views;



public partial class SkiaView : SKCanvasView, ISkiaDrawable
{



    public bool IsHardwareAccelerated => false;

    public void SignalFrame(long nanoseconds)
    { }

    public SKSurface CreateStandaloneSurface(int width, int height)
    {
        return SKSurface.Create(new SKImageInfo(width, height));
    }

    public Func<SKSurface, SKRect, bool> OnDraw { get; set; }

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
    private double _reportFps;


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
            return _reportFps;
        }
    }


    private double _fpsAverage;
    private int _fpsCount;
    private long _lastFrameTimestamp;

    /// <summary>
    /// Calculates the frames per second (FPS) and updates the rolling average FPS every 'averageAmount' frames.
    /// </summary>
    /// <param name="currentTimestamp">The current timestamp in nanoseconds.</param>
    /// <param name="averageAmount">The number of frames over which to average the FPS. Default is 10.</param>
    void CalculateFPS(long currentTimestamp, int averageAmount = 10)
    {
        // Convert nanoseconds to seconds for elapsed time calculation.
        double elapsedSeconds = (currentTimestamp - _lastFrameTimestamp) / 1_000_000_000.0;
        _lastFrameTimestamp = currentTimestamp;

        double currentFps = 1.0 / elapsedSeconds;

        _fpsAverage = ((_fpsAverage * _fpsCount) + currentFps) / (_fpsCount + 1);
        _fpsCount++;

        if (_fpsCount >= averageAmount)
        {
            _reportFps = _fpsAverage;
            _fpsCount = 0;
            _fpsAverage = 0.0;
        }
    }

    public long FrameTime { get; protected set; }

    public bool IsDrawing { get; protected set; }

    private void OnPaintingSurface(object sender, SKPaintSurfaceEventArgs paintArgs)
    {
        IsDrawing = true;

        FrameTime = Super.GetCurrentTimeNanos();

#if ANDROID
        CalculateFPS(FrameTime);
#else
        CalculateFPS(FrameTime, 60);
#endif

        if (OnDraw != null && Super.EnableRendering)
        {
            _surface = paintArgs.Surface;
            bool isDirty = OnDraw.Invoke(paintArgs.Surface, new SKRect(0, 0, paintArgs.Info.Width, paintArgs.Info.Height));

#if ANDROID
            if (maybeLowEnd && FPS > 200)
            {
                maybeLowEnd = false;
            }

            if (maybeLowEnd && isDirty && _fps < 55) //kick refresh for low-end devices
            {
                InvalidateSurface();
                return;
            }
#endif

        }

        IsDrawing = false;
    }

    static bool maybeLowEnd = true;

    public void Update(long nanos)
    {
        if (
            Super.EnableRendering &&
            this.Handler != null && this.Handler.PlatformView != null && CanvasSize is { Width: > 0, Height: > 0 })
        {
            IsDrawing = true;
            InvalidateSurface();
        }
    }


}
