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

        public MyOrientationListener(SkiaViewAccelerated parent, SkiaSharp.Views.Maui.Handlers.SKGLViewHandler renderer, Android.Hardware.SensorDelay rate) : base(renderer.Context, rate)
        {
            _owner = parent;
        }

        public override void OnOrientationChanged(int rotation)
        {
            _owner.Superview.SetDeviceOrientation(rotation);
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

            var stop = Handler;

            var renderer = Handler as SkiaSharp.Views.Maui.Handlers.SKGLViewHandler;
            var nativeView = renderer.PlatformView as SkiaSharp.Views.Android.SKGLTextureView;
            _orientationListener = new MyOrientationListener(this, renderer, Android.Hardware.SensorDelay.Normal);
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
            return _fps;
        }
    }

    public bool IsDrawing { get; set; }

    public double FrameTime { get; protected set; }

    /// <summary>
    /// We are drawing the frame
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="paintArgs"></param>
    private void OnPaintingSurface(object sender, SKPaintGLSurfaceEventArgs paintArgs)
    {
        IsDrawing = true;

        _fps = 1.0 / (DateTime.Now - _lastFrame).TotalSeconds;
        _lastFrame = DateTime.Now;

        FrameTime = Super.GetCurrentTimeNanos();

        if (OnDraw != null && Super.EnableRendering)
        {
            var rect = new SKRect(0, 0, paintArgs.BackendRenderTarget.Width, paintArgs.BackendRenderTarget.Height);
            _surface = paintArgs.Surface;
            var invalidate = OnDraw.Invoke(paintArgs.Surface.Canvas, rect);
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




