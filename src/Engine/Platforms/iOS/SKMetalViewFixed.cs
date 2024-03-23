using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Metal;
using MetalKit;
using ObjCRuntime;
using SkiaSharp.Views.iOS;

namespace DrawnUi.Maui.Views;

/// <summary>To be added.</summary>
/// <remarks>To be added.</remarks>
[Register("SKMetalViewFixed")]
[DesignTimeVisible(true)]
public class SKMetalViewFixed : MTKView, IMTKViewDelegate, INativeObject, IDisposable, IComponent
{
    private bool designMode;
    private GRMtlBackendContext backendContext;
    private GRContext context;

    private event EventHandler DisposedInternal;

    ISite IComponent.Site { get; set; }

    event EventHandler IComponent.Disposed
    {
        add => this.DisposedInternal += value;
        remove => this.DisposedInternal -= value;
    }

    public SKMetalViewFixed()
        : this(CGRect.Empty)
    {
        this.Initialize();
    }

    public SKMetalViewFixed(CGRect frame)
        : base(frame, (IMTLDevice) null)
    {
        this.Initialize();
    }

    public SKMetalViewFixed(CGRect frame, IMTLDevice device)
        : base(frame, device)
    {
        this.Initialize();
    }

    public SKMetalViewFixed(IntPtr p)
        : base((NativeHandle) p)
    {
    }

    /// <summary>To be added.</summary>
    /// <remarks>To be added.</remarks>
    public override void AwakeFromNib() => this.Initialize();

    private void Initialize()
    {
        ISite site = ((IComponent) this).Site;
        this.designMode = (site != null ? (site.DesignMode ? 1 : 0) : 0) != 0
                          || !true;//SkiaSharp.Views.iOS.Extensions.IsValidEnvironment;
        
        if (this.designMode)
            return;
        IMTLDevice systemDefault = MTLDevice.SystemDefault;
        if (systemDefault == null)
        {
            Console.WriteLine("Metal is not supported on this device.");
        }
        else
        {
            this.ColorPixelFormat = MTLPixelFormat.BGRA8Unorm;
            this.DepthStencilPixelFormat = MTLPixelFormat.Depth32Float_Stencil8;
            this.SampleCount = new UIntPtr(1);
            this.Device = systemDefault;
            this.FramebufferOnly = false; //fixes bug with flush/snapshot returning magenta color
            this.backendContext = new GRMtlBackendContext()
            {
                Device = systemDefault,
                Queue = systemDefault.CreateCommandQueue()
            };
            this.Delegate = (IMTKViewDelegate) this;
        }
    }

    /// <summary>To be added.</summary>
    /// <value>To be added.</value>
    /// <remarks>To be added.</remarks>
    public SKSize CanvasSize { get; private set; }

    /// <summary>To be added.</summary>
    /// <value>To be added.</value>
    /// <remarks>To be added.</remarks>
    public GRContext GRContext => this.context;

    /// <param name="view">To be added.</param>
    /// <param name="size">To be added.</param>
    /// <summary>To be added.</summary>
    /// <remarks>To be added.</remarks>
    void IMTKViewDelegate.DrawableSizeWillChange(MTKView view, CGSize size)
    {
        this.CanvasSize = size.ToSKSize();
        if (!this.Paused || !this.EnableSetNeedsDisplay)
            return;
        this.SetNeedsDisplay();
    }

    /// <param name="view">To be added.</param>
    /// <summary>To be added.</summary>
    /// <remarks>To be added.</remarks>
    void IMTKViewDelegate.Draw(MTKView view)
    {
        if (this.designMode || this.backendContext.Device == null || this.backendContext.Queue == null || this.CurrentDrawable?.Texture == null)
            return;
        this.CanvasSize = this.DrawableSize.ToSKSize();
        if ((double) this.CanvasSize.Width <= 0.0)
            return;
        SKSize canvasSize = this.CanvasSize;
        if ((double) canvasSize.Height <= 0.0)
            return;
        if (this.context == null)
            this.context = GRContext.CreateMetal(this.backendContext);
        GRMtlTextureInfo grMtlTextureInfo = new GRMtlTextureInfo(this.CurrentDrawable.Texture);
        canvasSize = this.CanvasSize;
        int width = (int) canvasSize.Width;
        canvasSize = this.CanvasSize;
        int height = (int) canvasSize.Height;
        int sampleCount = (int) this.SampleCount;
        GRMtlTextureInfo mtlInfo = grMtlTextureInfo;
        using (GRBackendRenderTarget renderTarget = new GRBackendRenderTarget(width, height, sampleCount, mtlInfo))
        {
            using (SKSurface surface = SKSurface.Create(this.context, renderTarget, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888))
            {
                using (SKCanvas canvas = surface.Canvas)
                {
                    this.OnPaintSurface(new SKPaintMetalSurfaceEventArgs(surface, renderTarget, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888));
                    canvas.Flush();
                    surface.Flush();
                    this.context.Flush();
                    using (IMTLCommandBuffer mtlCommandBuffer = this.backendContext.Queue.CommandBuffer())
                    {
                        mtlCommandBuffer.PresentDrawable((IMTLDrawable) this.CurrentDrawable);
                        mtlCommandBuffer.Commit();
                    }
                }
            }
        }
    }

    /// <summary>To be added.</summary>
    /// <remarks>To be added.</remarks>
    public event EventHandler<SKPaintMetalSurfaceEventArgs> PaintSurface;

    /// <param name="e">To be added.</param>
    /// <summary>To be added.</summary>
    /// <remarks>To be added.</remarks>
    protected virtual void OnPaintSurface(SKPaintMetalSurfaceEventArgs e)
    {
        EventHandler<SKPaintMetalSurfaceEventArgs> paintSurface = this.PaintSurface;
        if (paintSurface == null)
            return;
        paintSurface((object) this, e);
    }
}