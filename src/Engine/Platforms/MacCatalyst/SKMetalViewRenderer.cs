using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Metal;
using MetalKit;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Platform;
using SkiaSharp.Views.iOS;
using UIKit;


//[assembly: ExportRenderer(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer))]
namespace DrawnUi.Maui.Views;

public class SKMetalViewRenderer : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<SkiaViewAccelerated, SKMetalView2>
{
    private SKTouchHandlerPublic touchHandler;

    public SKMetalViewRenderer()
    {
        Initialize();

        SetDisablesUserInteraction(true);
    }

    private CoreAnimation.CADisplayLink displayLink;

    protected override SKMetalView2 CreateNativeControl()
    {
        var view = new SKMetalView2();

        // Force the opacity to false for consistency with the other platforms
        view.Opaque = false;

        //do not need that constant refresh! we gonna use InvalidateSurface
        view.EnableSetNeedsDisplay = true;

        return view;
    }

    private void Initialize()
    {
        touchHandler = new (
            args => ((ISKGLViewController)Element).OnTouch(args),
            (x, y) => GetScaledCoord(x, y));
    }

    public GRContext GRContext => Control.GRContext;

  
    protected virtual void SetupRenderLoop(bool oneShot)
    {
        // only start if we haven't already
        if (displayLink != null)
            return;

        // bail out if we are requesting something that the view doesn't want to
        if (!oneShot && !Element.HasRenderLoop)
            return;

        // if this is a one shot request, don't bother with the display link
        if (oneShot)
        {
            var nativeView = Control;
            nativeView?.BeginInvokeOnMainThread(() =>
            {
                if (nativeView.Handle != IntPtr.Zero)
                    nativeView.SetNeedsDisplay();
            });
            return;
        }

        // create the loop
        displayLink = CoreAnimation.CADisplayLink.Create(() =>
        {
            var nativeView = Control;
            var formsView = Element;

            // stop the render loop if this was a one-shot, or the views are disposed
            if (nativeView == null || formsView == null || nativeView.Handle == IntPtr.Zero || !formsView.HasRenderLoop)
            {
                displayLink.Invalidate();
                displayLink.Dispose();
                displayLink = null;
                return;
            }

            // redraw the view
            nativeView.SetNeedsDisplay();
        });

        displayLink.AddToRunLoop(Foundation.NSRunLoop.Current, Foundation.NSRunLoopMode.Default);
    }

    protected void SetDisablesUserInteraction(bool disablesUserInteraction)
    {
        touchHandler.DisablesUserInteraction = disablesUserInteraction;
    }
 

    protected override void OnElementChanged(ElementChangedEventArgs<SkiaViewAccelerated> e)
    {
        if (e.OldElement != null)
        {
            var oldController = (ISKGLViewController)e.OldElement;

            // unsubscribe from events
            oldController.SurfaceInvalidated -= OnSurfaceInvalidated;
            oldController.GetCanvasSize -= OnGetCanvasSize;
            oldController.GetGRContext -= OnGetGRContext;
        }

        if (e.NewElement != null)
        {
            var newController = (ISKGLViewController)e.NewElement;

            // create the native view
            if (Control == null)
            {
                var view = CreateNativeControl();

                view.PaintSurface += OnPaintSurface;

                SetNativeControl(view);
            }

            touchHandler.SetEnabled(Control, e.NewElement.EnableTouchEvents);

            // subscribe to events from the user
            newController.SurfaceInvalidated += OnSurfaceInvalidated;
            newController.GetCanvasSize += OnGetCanvasSize;
            newController.GetGRContext += OnGetGRContext;

            // start the rendering
            SetupRenderLoop(false);
        }

        base.OnElementChanged(e);
    }



    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnElementPropertyChanged(sender, e);

        // refresh the render loop
        if (e.PropertyName == SKGLView.HasRenderLoopProperty.PropertyName)
        {
            SetupRenderLoop(false);
        }
        else if (e.PropertyName == SKGLView.EnableTouchEventsProperty.PropertyName)
        {
            touchHandler.SetEnabled(Control, Element.EnableTouchEvents);
        }
 
    }

    protected override void Dispose(bool disposing)
    {
        // stop the render loop
        if (displayLink != null)
        {
            displayLink.Invalidate();
            displayLink.Dispose();
            displayLink = null;
        }

        // detach all events before disposing
        var controller = (ISKGLViewController)Element;
        if (controller != null)
        {
            controller.SurfaceInvalidated -= OnSurfaceInvalidated;
            controller.GetCanvasSize -= OnGetCanvasSize;
            controller.GetGRContext -= OnGetGRContext;
        }

        var control = Control;
        if (control != null)
        {
            control.PaintSurface -= OnPaintSurface;
        }

        // detach, regardless of state
        touchHandler.Detach(control);

        base.Dispose(disposing);
    }

    //protected abstract void SetupRenderLoop(bool oneShot);

    private SKPoint GetScaledCoord(double x, double y)
    {

        x = x * Control.ContentScaleFactor;
        y = y * Control.ContentScaleFactor;

        //catalyst
        //x = x * Control.Window.BackingScaleFactor;
		//y = y * Control.Window.BackingScaleFactor;

        return new SKPoint((float)x, (float)y);
    }

    // the user asked to repaint
    private void OnSurfaceInvalidated(object sender, EventArgs eventArgs)
    {
        // if we aren't in a loop, then refresh once
        if (!Element.HasRenderLoop)
        {
            SetupRenderLoop(true);
        }
    }

    // the user asked for the size
    private void OnGetCanvasSize(object sender, GetPropertyValueEventArgs<SKSize> e)
    {
        e.Value = Control?.CanvasSize ?? SKSize.Empty;
    }

    // the user asked for the current GRContext
    private void OnGetGRContext(object sender, GetPropertyValueEventArgs<GRContext> e)
    {
        e.Value = Control?.GRContext;
    }

    private void OnPaintSurface(object sender, SkiaSharp.Views.iOS.SKPaintMetalSurfaceEventArgs e)
    {
        var controller = Element as ISKGLViewController;

        // the control is being repainted, let the user know
        controller?.OnPaintSurface(new SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs(e.Surface, e.BackendRenderTarget));
    }
}

public class SKTouchHandlerPublic : UIKit.UIGestureRecognizer
{
    private Action<SKTouchEventArgs>? onTouchAction;
    private Func<double, double, SKPoint>? scalePixels;

    public SKTouchHandlerPublic(Action<SKTouchEventArgs> onTouchAction, Func<double, double, SKPoint> scalePixels)
    {
        this.onTouchAction = onTouchAction;
        this.scalePixels = scalePixels;

        DisablesUserInteraction = false;
    }

    public bool DisablesUserInteraction { get; set; }

    public void SetEnabled(UIKit.UIView view, bool enableTouchEvents)
    {
        if (view != null)
        {
            if (!view.UserInteractionEnabled || DisablesUserInteraction)
            {
                view.UserInteractionEnabled = enableTouchEvents;
            }
            if (enableTouchEvents && view.GestureRecognizers?.Contains(this) != true)
            {
                view.AddGestureRecognizer(this);
            }
            else if (!enableTouchEvents && view.GestureRecognizers?.Contains(this) == true)
            {
                view.RemoveGestureRecognizer(this);
            }
        }
    }

    public void Detach(UIKit.UIView view)
    {
        // clean the view
        SetEnabled(view, false);

        // remove references
        onTouchAction = null;
        scalePixels = null;
    }

    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
        base.TouchesBegan(touches, evt);

        foreach (UIKit.UITouch touch in touches.Cast<UIKit.UITouch>())
        {
            if (!FireEvent(SKTouchAction.Pressed, touch, true))
            {
                IgnoreTouch(touch, evt);
            }
        }
    }

    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
        base.TouchesMoved(touches, evt);

        foreach (UIKit.UITouch touch in touches.Cast<UIKit.UITouch>())
        {
            FireEvent(SKTouchAction.Moved, touch, true);
        }
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
        base.TouchesEnded(touches, evt);

        foreach (UIKit.UITouch touch in touches.Cast<UITouch>())
        {
            FireEvent(SKTouchAction.Released, touch, false);
        }
    }

    public override void TouchesCancelled(NSSet touches, UIEvent evt)
    {
        base.TouchesCancelled(touches, evt);

        foreach (UITouch touch in touches.Cast<UITouch>())
        {
            FireEvent(SKTouchAction.Cancelled, touch, false);
        }
    }

    private bool FireEvent(SKTouchAction actionType, UITouch touch, bool inContact)
    {
        if (onTouchAction == null || scalePixels == null)
            return false;

        var id = ((IntPtr)touch.Handle).ToInt64();

        var cgPoint = touch.LocationInView(View);
        var point = scalePixels(cgPoint.X, cgPoint.Y);

        var args = new SKTouchEventArgs(id, actionType, point, inContact);
        onTouchAction(args);
        return args.Handled;
    }
}

public class SKMetalView2 : MTKView, IMTKViewDelegate, IComponent
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

    // created in code
    public SKMetalView2()
        : this(CGRect.Empty)
    {
        Initialize();
    }

    // created in code
    public SKMetalView2(CGRect frame)
        : base(frame, null)
    {
        Initialize();
    }

    // created in code
    public SKMetalView2(CGRect frame, IMTLDevice device)
        : base(frame, device)
    {
        Initialize();
    }

    // created via designer
    public SKMetalView2(IntPtr p)
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
        designMode = ((IComponent)this).Site?.DesignMode == true;

        if (designMode)
            return;

        var device = MTLDevice.SystemDefault;
        if (device == null)
        {
            Trace.WriteLine("Metal is not supported on this device.");
            return;
        }

        ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
        DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;
        SampleCount = 1;
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

        if (Paused && EnableSetNeedsDisplay)

        SetNeedsDisplay();
    }

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
        context ??= GRContext.CreateMetal(backendContext); //always returns null

        if (context == null)
        {
            return;
        }

        const SKColorType colorType = SKColorType.Bgra8888;
        const GRSurfaceOrigin surfaceOrigin = GRSurfaceOrigin.TopLeft;

        // create the render target
        var metalInfo = new GRMtlTextureInfo(CurrentDrawable.Texture);
        using var renderTarget = new GRBackendRenderTarget((int)CanvasSize.Width, (int)CanvasSize.Height, (int)SampleCount, metalInfo);

        // create the surface
        using var surface = SKSurface.Create(context, renderTarget, surfaceOrigin, colorType);
        using var canvas = surface.Canvas;

        // start drawing
        var e = new SKPaintMetalSurfaceEventArgs(surface, renderTarget, surfaceOrigin, colorType);
        OnPaintSurface(e);

        // flush the SkiaSharp contents
        canvas.Flush();
        surface.Flush();
        context.Flush();

        // present
        using var commandBuffer = backendContext.Queue.CommandBuffer();
        commandBuffer.PresentDrawable(CurrentDrawable);
        commandBuffer.Commit();
    }

    public event EventHandler<SKPaintMetalSurfaceEventArgs> PaintSurface;

    protected virtual void OnPaintSurface(SKPaintMetalSurfaceEventArgs e)
    {
        PaintSurface?.Invoke(this, e);
    }
}




