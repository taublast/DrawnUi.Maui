using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace DrawnUi.Views;

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

        public MyOrientationListener(SkiaViewAccelerated parent, SKGLViewRenderer renderer, Android.Hardware.SensorDelay rate) : base(renderer.Context, rate)
        //public MyOrientationListener(SkiaViewAccelerated parent, SkiaSharp.Views.Maui.Handlers.SKGLViewHandler renderer, Android.Hardware.SensorDelay rate) : base(renderer.Context, rate)
        {
            _owner = parent;
        }

        public override void OnOrientationChanged(int rotation)
        {
            _owner.Superview?.SetDeviceOrientation(rotation);
        }
    }


#endif

    bool rendererSet;

    protected void OnHandlerChanged()
    {
        if (rendererSet)
        {
            //disconnect
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
            rendererSet = true;
            //connect
            PaintSurface -= OnPaintingSurface;
            PaintSurface += OnPaintingSurface;

#if ANDROID

            var renderer = Handler as SkiaSharp.Views.Maui.Controls.Compatibility.SKGLViewRenderer;
            var nativeView = renderer.Control as SkiaSharp.Views.Android.SKGLTextureView;
            //var renderer = Handler as SkiaSharp.Views.Maui.Handlers.SKGLViewHandler;
            //var nativeView = renderer.PlatformView as SkiaSharp.Views.Android.SKGLTextureView;
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

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == "Renderer")
        {
            OnHandlerChanged();
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
            return _reportFps;
        }
    }

    public bool IsDrawing { get; set; }

    public long FrameTime { get; protected set; }


    public void Update()
    {
        if (
            Super.EnableRendering && rendererSet && CanvasSize is { Width: > 0, Height: > 0 })
        {
            IsDrawing = true;
            InvalidateSurface();
        }
    }


    private double _fpsAverage;
    private int _fpsCount;
    private long _lastFrameTimestamp;
    private long _nanos;
    private bool _isDrawing;
    static bool maybeLowEnd = true;
    private double _reportFps;

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


    /// <summary>
    /// We are drawing the frame
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="paintArgs"></param>
    private void OnPaintingSurface(object sender, SKPaintGLSurfaceEventArgs paintArgs)
    {
        IsDrawing = true;

        FrameTime = Super.GetCurrentTimeNanos();

        if (Device.RuntimePlatform == Device.Android)
        {
            CalculateFPS(FrameTime);
        }
        else
        {
            CalculateFPS(FrameTime, 60);
        }

        if (OnDraw != null && Super.EnableRendering)
        {
            var rect = new SKRect(0, 0, paintArgs.BackendRenderTarget.Width, paintArgs.BackendRenderTarget.Height);
            _surface = paintArgs.Surface;
            var isDirty = OnDraw.Invoke(paintArgs.Surface.Canvas, rect);

            if (Device.RuntimePlatform == Device.Android)
            {
                if (maybeLowEnd && FPS > 60)
                {
                    maybeLowEnd = false;
                }

                if (maybeLowEnd && isDirty && _fps < 30) //kick refresh for low-end devices
                {
                    InvalidateSurface();
                    return;
                }
            }
        }

        IsDrawing = false;
    }


}




