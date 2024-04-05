namespace DrawnUi.Maui.Views;

public partial class SkiaViewAccelerated : SKGLView, ISkiaDrawable
{

    public SKSurface CreateStandaloneSurface(int width, int height)
    {
        return SKSurface.Create(new SKImageInfo(width, height));
    }

    public Func<SKCanvas, SKRect, bool> OnDraw { get; set; }

    public SkiaViewAccelerated(DrawnView superview)
    {
        Superview = superview;
        EnableTouchEvents = false;
    }



#if ANDROID

    private MyOrientationListener _orientationListener;

    public class MyOrientationListener : Android.Views.OrientationEventListener
    {
        private SkiaViewAccelerated _owner;

        public MyOrientationListener(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MyOrientationListener(Android.Content.Context context) : base(context)
        {
        }

        public MyOrientationListener(SkiaViewAccelerated parent, Android.Hardware.SensorDelay rate) : base(Platform.AppContext, rate)
        {
            _owner = parent;
        }

        public override void OnOrientationChanged(int rotation)
        {
            _owner.Superview?.SetDeviceOrientation(rotation);
        }
    }


#endif

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler == null)
        {
            PaintSurface -= OnPaintingSurface;

#if ANDROID

            if (_orientationListener != null)
            {
                _orientationListener.Disable();
                _orientationListener.Dispose();
                _orientationListener = null;
            }

#endif

            Superview?.DisconnectedHandler();
        }
        else
        {
            PaintSurface -= OnPaintingSurface;
            PaintSurface += OnPaintingSurface;

#if ANDROID

            var renderer = Handler;// as SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer;
                                   //var nativeView = renderer.Control as SkiaSharp.Views.Android.SKGLTextureView;
                                   //var renderer = Handler as SkiaSharp.Views.Maui.Handlers.SKGLViewHandler;
                                   //var nativeView = renderer.PlatformView as SkiaSharp.Views.Android.SKGLTextureView;

            _orientationListener = new MyOrientationListener(this, Android.Hardware.SensorDelay.Normal);
            if (_orientationListener.CanDetectOrientation())
                _orientationListener.Enable();

#elif IOS

            if (DeviceInfo.Current.DeviceType != DeviceType.Virtual)
            {
                //var renderer = Handler as SKMetalViewRenderer;
                //var nativeView = renderer.Control as SkiaSharp.Views.iOS.SKMetalView;
            }
            else
            {
                //var renderer = Handler as SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer;
                //var nativeView = renderer.Control as SkiaSharp.Views.iOS.SKGLView;
            }

            //#elif MACCATALYST

            //            var renderer = Handler as SKMetalViewRenderer;
            //            var nativeView = renderer.Control as SkiaSharp.Views.iOS.SKMetalView;

#endif

            Superview?.ConnectedHandler();
        }

    }

    public DrawnView Superview { get; protected set; }
    private SKImage _snapshot;
    private bool _newFrameReady;

    public void Dispose()
    {
        PaintSurface -= OnPaintingSurface;
        _surface = null;
        _snapshot?.Dispose();
        _snapshot = null;
        Superview = null;
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

    public bool IsHardwareAccelerated => true;

    public double FPS
    {
        get
        {
            return _reportFps;
        }
    }

    public bool IsDrawing { get; set; }

    public long FrameTime { get; protected set; }

    public void SignalFrame(long nanoseconds)
    {

    }

    public void Update(long nanos)
    {
        if (
            Super.EnableRendering &&
            this.Handler != null && this.Handler.PlatformView != null && CanvasSize is { Width: > 0, Height: > 0 })
        {
            _nanos = nanos;
            IsDrawing = true;
            InvalidateSurface();
        }
    }
    
    private double _fpsAverage;
    private int _fpsCount;
    private long _lastFrameTimestamp;
    private long _nanos;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentTimestamp">Nanoseconds</param>
    void CalculateFPS(long currentTimestamp, int averageAmount = 10)
    {
        double elapsedSeconds = (currentTimestamp - _lastFrameTimestamp) / 1_000_000_000.0; // Convert nanoseconds to seconds
        double fps = 1.0 / elapsedSeconds;
    
        _lastFrameTimestamp = currentTimestamp;
    
        _fpsAverage += fps;
        _fpsCount++;

        if (_fpsCount >= averageAmount) 
        {
            _reportFps = _fpsAverage / _fpsCount;

            _fpsCount = 0;
            _fpsAverage = 0.0;
        }
    }
    
    /// <summary>
    /// We are drawing the frame
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="paintArgs"></param>
    private void OnPaintingSurface(object sender, SKPaintGLSurfaceEventArgs paintArgs)
    {
        IsDrawing = true;
        
        FrameTime = Super.GetCurrentTimeNanos();

        CalculateFPS(FrameTime);
       
        if (OnDraw != null && Super.EnableRendering)
        {
            var rect = new SKRect(0, 0, paintArgs.BackendRenderTarget.Width, paintArgs.BackendRenderTarget.Height);
            _surface = paintArgs.Surface;
            var isDirty = OnDraw.Invoke(paintArgs.Surface.Canvas, rect);
//#if ANDROID
            if (isDirty && FPS < 60) //fix refresh for low-end devices, gamechanger
            {
                InvalidateSurface();
                return;
            }
//#endif
        }

        IsDrawing = false;
    }

}




