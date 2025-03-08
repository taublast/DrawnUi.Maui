using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using UIKit;

namespace DrawnUi.Maui.Views;

public class MauiSKMetalViewFixed : SKMetalViewFixed
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

public partial class SKGLViewHandlerFixed
{
    public static PropertyMapper<ISKGLView, SKGLViewHandlerFixed> SKGLViewMapper =
        new PropertyMapper<ISKGLView, SKGLViewHandlerFixed>(ViewHandler.ViewMapper)
        {
            [nameof(ISKGLView.EnableTouchEvents)] = MapEnableTouchEvents,
            [nameof(ISKGLView.IgnorePixelScaling)] = MapIgnorePixelScaling,
            [nameof(ISKGLView.HasRenderLoop)] = MapHasRenderLoop,
#if WINDOWS
				[nameof(ISKGLView.Background)] = MapBackground,
#endif
        };

    public static CommandMapper<ISKGLView, SKGLViewHandlerFixed> SKGLViewCommandMapper =
        new CommandMapper<ISKGLView, SKGLViewHandlerFixed>(ViewHandler.ViewCommandMapper)
        {
            [nameof(ISKGLView.InvalidateSurface)] = OnInvalidateSurface,
        };

    public SKGLViewHandlerFixed()
        : base(SKGLViewMapper, SKGLViewCommandMapper)
    {
    }

    public SKGLViewHandlerFixed(PropertyMapper? mapper, CommandMapper? commands)
        : base(mapper ?? SKGLViewMapper, commands ?? SKGLViewCommandMapper)
    {
    }
}

public partial class SKGLViewHandlerFixed : ViewHandler<ISKGLView, SKMetalViewFixed>
{
    private PaintSurfaceProxy? paintSurfaceProxy;

    //todo restore
    //private SKTouchHandlerProxy? touchProxy;

    protected override SKMetalViewFixed CreatePlatformView() =>
        new MauiSKMetalViewFixed
        {
            BackgroundColor = UIColor.Clear,
            Opaque = false,
        };

    protected override void ConnectHandler(SKMetalViewFixed platformView)
    {
        paintSurfaceProxy = new();
        paintSurfaceProxy.Connect(VirtualView, platformView);

        //todo restore
        //touchProxy = new();
        //touchProxy.Connect(VirtualView, platformView);

        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(SKMetalViewFixed platformView)
    {
        paintSurfaceProxy?.Disconnect(platformView);
        paintSurfaceProxy = null;

        //todo restore
        //touchProxy?.Disconnect(platformView);
        //touchProxy = null;

        base.DisconnectHandler(platformView);
    }

    // Mapper actions / properties

    public static void OnInvalidateSurface(SKGLViewHandlerFixed handler, ISKGLView view, object? args)
    {
        if (handler.PlatformView.ManualRefresh)
            handler.PlatformView.SetNeedsDisplay();
    }

    public static void MapIgnorePixelScaling(SKGLViewHandlerFixed handler, ISKGLView view)
    {
        if (handler.PlatformView is MauiSKMetalViewFixed pv)
        {
            pv.IgnorePixelScaling = view.IgnorePixelScaling;
            handler.PlatformView.SetNeedsDisplay();
        }
    }

    public static void MapHasRenderLoop(SKGLViewHandlerFixed handler, ISKGLView view)
    {
        handler.PlatformView.Paused = !view.HasRenderLoop;
        handler.PlatformView.EnableSetNeedsDisplay = !view.HasRenderLoop;
    }

    public static void MapEnableTouchEvents(SKGLViewHandlerFixed handler, ISKGLView view)
    {
        //todo restore
        //handler.touchProxy?.UpdateEnableTouchEvents(handler.PlatformView, view.EnableTouchEvents);
    }

    // helper methods

    private class MauiSKMetalViewFixed : SKMetalViewFixed
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

    private class PaintSurfaceProxy : SKEventProxy<ISKGLView, SKMetalViewFixed>
    {
        private SKSizeI lastCanvasSize;
        private GRContext? lastGRContext;

        protected override void OnConnect(ISKGLView virtualView, SKMetalViewFixed platformView) =>
            platformView.PaintSurface += OnPaintSurface;

        protected override void OnDisconnect(SKMetalViewFixed platformView) =>
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
            if (sender is SKMetalViewFixed platformView)
            {
                var newGRContext = platformView.GRContext;
                if (lastGRContext != newGRContext)
                {
                    lastGRContext = newGRContext;
                    view.OnGRContextChanged(newGRContext);
                }
            }

            view.OnPaintSurface(new SKPaintGLSurfaceEventArgs(e.Surface, e.BackendRenderTarget, e.Origin, e.Info, e.RawInfo));
        }
    }
}
