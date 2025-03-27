using System.ComponentModel;
using CoreGraphics;
using Foundation;
using Metal;
using MetalKit;
using ObjCRuntime;
using SkiaSharp.Views.iOS;

namespace DrawnUi.Maui.Views
{

    [Register(nameof(SKMetalViewRetained))]
    [DesignTimeVisible(true)]
    public class SKMetalViewRetained : MTKView, IMTKViewDelegate, IComponent
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
        public SKMetalViewRetained()
            : this(CGRect.Empty)
        {
            Initialize();
        }

        // created in code
        public SKMetalViewRetained(CGRect frame)
            : base(frame, null)
        {
            Initialize();
        }

        // created in code
        public SKMetalViewRetained(CGRect frame, IMTLDevice device)
            : base(frame, device)
        {
            Initialize();
        }

        // created via designer
        public SKMetalViewRetained(IntPtr p)
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


        public override void Draw()
        {
            base.Draw();

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
            context ??= GRContext.CreateMetal(backendContext);

            const SKColorType colorType = SKColorType.Bgra8888;
            const GRSurfaceOrigin surfaceOrigin = GRSurfaceOrigin.TopLeft;

            bool needsNewSurface = _surface == null ||
                                   _lastTarget == null ||
                                   !_lastTarget.IsValid ||
                                   _lastTargetInfo != CanvasSize;

            if (needsNewSurface)
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
            commandBuffer.AddCompletedHandler(buffer => {

                // GPU finished rendering
                // runs on background thread




            });
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
}
