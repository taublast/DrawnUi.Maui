using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Metal;
using MetalKit;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using UIKit;
using SkiaSharp.Views.iOS;

namespace DrawnUi.Maui.Views;

/// <summary>
/// APPLE
/// </summary>
public class MauiSkMetalViewRetained : SKMetalViewFixed
{
    public bool IgnorePixelScaling { get; set; }

    protected override void OnPaintSurface(SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs e)
    {
        if (IgnorePixelScaling)
        {
            var userVisibleSize = new SKSizeI((int)Bounds.Width, (int)Bounds.Height);
            var canvas = e.Surface.Canvas;
            canvas.Scale((float)ContentScaleFactor);
            canvas.Save();

            e = new SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs(e.Surface, e.BackendRenderTarget, e.Origin, e.Info.WithSize(userVisibleSize), e.Info);
        }

        base.OnPaintSurface(e);
    }
}

/// <summary>
/// APPLE
/// </summary>
[Register(nameof(SKMetalViewFixed))]
[DesignTimeVisible(true)]
public class SKMetalViewFixed : MTKView, IMTKViewDelegate, IComponent
{
    // for IComponent
#pragma warning disable 67
    private event EventHandler DisposedInternal;
#pragma warning restore 67
    ISite IComponent.Site { get; set; }
    event EventHandler IComponent.Disposed
    {
        add { DisposedInternal += value; }
        remove { DisposedInternal -= value; }
    }

    private bool designMode;

    private GRMtlBackendContext backendContext;
    private GRContext context;

    public bool ManualRefresh
    {
        get
        {
            return Paused && EnableSetNeedsDisplay;
        }
    }

    // created in code
    public SKMetalViewFixed()
        : this(CGRect.Empty)
    {
        Initialize();
    }

    // created in code
    public SKMetalViewFixed(CGRect frame)
        : base(frame, null)
    {
        Initialize();
    }

    // created in code
    public SKMetalViewFixed(CGRect frame, IMTLDevice device)
        : base(frame, device)
    {
        Initialize();
    }

    // created via designer
    public SKMetalViewFixed(IntPtr p)
        : base(p)
    {
    }

    // created via designer
    public override void AwakeFromNib()
    {
        Initialize();
    }

    private void Initialize()
    {
        designMode = ((IComponent)this).Site?.DesignMode == true;// || !EnvironmentExtensions.IsValidEnvironment;

        if (designMode)
            return;

        var device = MTLDevice.SystemDefault;
        if (device == null)
        {
            Console.WriteLine("Metal is not supported on this device.");
            return;
        }

        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;

        //https://developer.apple.com/documentation/metal/developing-metal-apps-that-run-in-simulator?language=objc
        //make simulator performant
        if (DeviceInfo.Current.DeviceType == DeviceType.Virtual)
        {
            DepthStencilStorageMode = MTLStorageMode.Private;
            SampleCount = 4;
        }
        else
        {
            DepthStencilStorageMode = MTLStorageMode.Shared;
            SampleCount = 2;
        }

        //gpu memory used NOT only for rendering
        //but could be read by skiasharp too
        FramebufferOnly = false;

        Device = device;
        backendContext = new GRMtlBackendContext
        {
            Device = device,
            Queue = device.CreateCommandQueue(),
        };

        // hook up the drawing
        Delegate = this;
    }

    public SKSize CanvasSize { get; private set; }

    public GRContext GRContext => context;

    void IMTKViewDelegate.DrawableSizeWillChange(MTKView view, CGSize size)
    {
        CanvasSize = size.ToSKSize();

        if (ManualRefresh)
            SetNeedsDisplay();
    }

    SKSurface _surface;
    GRBackendRenderTarget _lastTarget;
    SKSize _lastTargetInfo;

    void IMTKViewDelegate.Draw(MTKView view)
    {
        if (designMode)
            return;

        if (backendContext.Device == null || backendContext.Queue == null || CurrentDrawable?.Texture == null)
            return;

        CanvasSize = DrawableSize.ToSKSize();

        if (CanvasSize.Width <= 0 || CanvasSize.Height <= 0)
            return;

        // create the contexts if not done already
        context ??= GRContext.CreateMetal(backendContext);

        const SKColorType colorType = SKColorType.Bgra8888;
        const GRSurfaceOrigin surfaceOrigin = GRSurfaceOrigin.TopLeft;

        if (_lastTarget != null && _lastTargetInfo == CanvasSize && _surface != null)
        {
            //reuse

        }
        else
        {
            DisposeTarget();

            _lastTargetInfo = CanvasSize;

            // create the render target
            var metalInfo = new GRMtlTextureInfo(CurrentDrawable.Texture);
            _lastTarget = new GRBackendRenderTarget((int)CanvasSize.Width, (int)CanvasSize.Height, (int)SampleCount, metalInfo);

            // create the surface
            _surface = SKSurface.Create(context, _lastTarget, surfaceOrigin, colorType);
        }

        // start drawing
        var e = new SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs(_surface, _lastTarget, surfaceOrigin, colorType);
        OnPaintSurface(e);

        // flush the SkiaSharp contents
        _surface.Canvas.Flush();
        _surface.Flush();
        context.Flush();

        // present
        using var commandBuffer = backendContext.Queue.CommandBuffer();
        commandBuffer.PresentDrawable(CurrentDrawable);
        commandBuffer.Commit();
    }

    void DisposeTarget()
    {
        _lastTarget?.Dispose();
        _surface?.Dispose();
    }

    public event EventHandler<SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs> PaintSurface;

    protected virtual void OnPaintSurface(SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs e)
    {
        PaintSurface?.Invoke(this, e);
    }
}


/// <summary>
/// APPLE
/// </summary>
/// <typeparam name="TVirtualView"></typeparam>
/// <typeparam name="TPlatformView"></typeparam>
internal class SKEventProxy<TVirtualView, TPlatformView>
    where TVirtualView : class
    where TPlatformView : class
{
    private WeakReference<TVirtualView>? virtualView;

    protected TVirtualView? VirtualView =>
        virtualView is not null && virtualView.TryGetTarget(out var v) ? v : null;

    public void Connect(TVirtualView virtualView, TPlatformView platformView)
    {
        this.virtualView = new(virtualView);
        OnConnect(virtualView, platformView);
    }

    protected virtual void OnConnect(TVirtualView virtualView, TPlatformView platformView)
    {
    }

    public void Disconnect(TPlatformView platformView)
    {
        virtualView = null;
        OnDisconnect(platformView);
    }

    protected virtual void OnDisconnect(TPlatformView platformView)
    {
    }
}


/// <summary>
/// APPLE Handler with Mapper (originally iOS)
/// </summary>
public partial class SKGLViewHandlerRetained : ViewHandler<ISKGLView, SKMetalViewRetained>
{
    private PaintSurfaceProxy? paintSurfaceProxy;

    //todo restore
    //private SKTouchHandlerProxy? touchProxy;

    protected override SKMetalViewRetained CreatePlatformView() =>
        new MauiSkMetalViewRetained
        {
            BackgroundColor = UIColor.Clear,
            Opaque = false,
        };

    protected override void ConnectHandler(SKMetalViewRetained platformView)
    {
        paintSurfaceProxy = new();
        paintSurfaceProxy.Connect(VirtualView, platformView);

        //todo restore
        //touchProxy = new();
        //touchProxy.Connect(VirtualView, platformView);

        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(SKMetalViewRetained platformView)
    {
        paintSurfaceProxy?.Disconnect(platformView);
        paintSurfaceProxy = null;

        //todo restore
        //touchProxy?.Disconnect(platformView);
        //touchProxy = null;

        base.DisconnectHandler(platformView);
    }

    // Mapper actions / properties

    public static void OnInvalidateSurface(SKGLViewHandlerRetained handler, ISKGLView view, object? args)
    {
        if (handler.PlatformView.ManualRefresh)
            handler.PlatformView.SetNeedsDisplay();
    }

    public static void MapIgnorePixelScaling(SKGLViewHandlerRetained handler, ISKGLView view)
    {
        if (handler.PlatformView is MauiSkMetalViewRetained pv)
        {
            pv.IgnorePixelScaling = view.IgnorePixelScaling;
            handler.PlatformView.SetNeedsDisplay();
        }
    }

    public static void MapHasRenderLoop(SKGLViewHandlerRetained handler, ISKGLView view)
    {
        handler.PlatformView.Paused = !view.HasRenderLoop;
        handler.PlatformView.EnableSetNeedsDisplay = !view.HasRenderLoop;
    }

    public static void MapEnableTouchEvents(SKGLViewHandlerRetained handler, ISKGLView view)
    {
        //todo restore
        //handler.touchProxy?.UpdateEnableTouchEvents(handler.PlatformView, view.EnableTouchEvents);
    }

    // helper methods

    private class MauiSkMetalViewRetained : SKMetalViewRetained
    {
        public bool IgnorePixelScaling { get; set; }

        protected override void OnPaintSurface(SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs e)
        {
            if (IgnorePixelScaling)
            {
                var userVisibleSize = new SKSizeI((int)Bounds.Width, (int)Bounds.Height);
                var canvas = e.Surface.Canvas;
                canvas.Scale((float)ContentScaleFactor);
                canvas.Save();

                e = new SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs(e.Surface, e.BackendRenderTarget, e.Origin, e.Info.WithSize(userVisibleSize), e.Info);
            }

            base.OnPaintSurface(e);
        }
    }

    private class PaintSurfaceProxy : SKEventProxy<ISKGLView, SKMetalViewRetained>
    {
        private SKSizeI lastCanvasSize;
        private GRContext? lastGRContext;

        protected override void OnConnect(ISKGLView virtualView, SKMetalViewRetained platformView) =>
            platformView.PaintSurface += OnPaintSurface;

        protected override void OnDisconnect(SKMetalViewRetained platformView) =>
            platformView.PaintSurface -= OnPaintSurface;

        private void OnPaintSurface(object? sender, SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs e)
        {
            if (VirtualView is not { } view)
                return;

            var newCanvasSize = e.Info.Size;
            if (lastCanvasSize != newCanvasSize)
            {
                lastCanvasSize = newCanvasSize;
                view.OnCanvasSizeChanged(newCanvasSize);
            }
            if (sender is SKMetalViewRetained platformView)
            {
                var newGRContext = platformView.GRContext;
                if (lastGRContext != newGRContext)
                {
                    lastGRContext = newGRContext;
                    view.OnGRContextChanged(newGRContext);
                }
            }

            view.OnPaintSurface(new (e.Surface, e.BackendRenderTarget, e.Origin, e.Info, e.RawInfo));
        }
    }



}

 


