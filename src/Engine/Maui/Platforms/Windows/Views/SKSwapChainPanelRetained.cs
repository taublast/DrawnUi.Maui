using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Application = Microsoft.Maui.Controls.Application;

namespace DrawnUi.Maui.Views
{
    /*

    Initialization (once):
          ├─ Initialize GRContext
          └─ Create persistent retained surface

       Every Frame:
          ├─ Check size changes (resize handling)
          ├─ Invoke user drawing onto retained surface
          ├─ Snapshot retained surface
          └─ Copy snapshot to actual framebuffer for display

       On context destroy:
          └─ Dispose GPU resources

     */

  

    public class SKSwapChainPanelRetained : AngleSwapChainPanel
    {
        public Guid Uid = Guid.NewGuid();

        private const SKColorType ColorType = SKColorType.Rgba8888;
        private const GRSurfaceOrigin SurfaceOrigin = GRSurfaceOrigin.BottomLeft;
        private GRGlInterface _glInterface;
        private GRContext _context;
        private SKSurface _retainedSurface; // Persistent offscreen surface
        private SKSizeI _lastSize;
        private bool _needsFullRedraw = true; // For initial frame or size change

        private readonly object _surfaceLock = new();

        public SKSwapChainPanelRetained()
        {
            _cleanupRunning = true;
            Tasks.StartTimerAsync(TimeSpan.FromSeconds(2), async () =>
            {
                while (_trashBag.TryTake(out var pooledSurface))
                {
                    // After a long delay, GPU is guaranteed done
                    pooledSurface.Dispose();
                    Debug.WriteLine("Disposed pooled surface safely after delay");
                }

                return _cleanupRunning;
            });
        }

        public SKSize CanvasSize => _lastSize;
        public GRContext GRContext => _context;
        public event EventHandler<SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs> PaintSurface;

        protected virtual void OnPaintSurface(SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs e)
        {
            PaintSurface?.Invoke(this, e);
        }


        protected override void OnRenderFrame(Windows.Foundation.Rect rect)
        {
            //lock (_surfaceLock)
            {
                if (_context == null)
                {
                    _glInterface = GRGlInterface.Create();
                    _context = GRContext.CreateGl(_glInterface);
                }

                var newSize = new SKSizeI((int)rect.Width, (int)rect.Height);

                bool needNewSurfaces = _retainedSurface == null || _lastSize != newSize;

                if (_lastSize != newSize || renderTarget == null || !renderTarget.IsValid)
                {
                    var previousTarget = renderTarget;
                    if (previousTarget != null)
                    {
                        Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromSeconds(2), () =>
                        {
                            previousTarget.Dispose();
                            return false;
                        });
                    }

                    OpenGles.GetIntegerv(OpenGles.GL_FRAMEBUFFER_BINDING, out var framebuffer);
                    OpenGles.GetIntegerv(OpenGles.GL_STENCIL_BITS, out var stencil);
                    OpenGles.GetIntegerv(OpenGles.GL_SAMPLES, out var samples);
                    var maxSamples = _context.GetMaxSurfaceSampleCount(ColorType);
                    samples = Math.Min(samples, maxSamples);
                    var glInfo = new GRGlFramebufferInfo((uint)framebuffer, ColorType.ToGlSizedFormat());

                    renderTarget =
                        new GRBackendRenderTarget(newSize.Width, newSize.Height, samples, stencil, glInfo);

                    needNewSurfaces = true;
                }

                if (needNewSurfaces)
                {

                    if (_retainedSurface != null)
                    {
                        Debug.WriteLine("Abandon surface to pool");
                        // Flush commands but defer disposal
                        _retainedSurface.Canvas.Flush();
                        _retainedSurface.Flush();
                        _context.Flush();

                        // Put the old surface into a recycling pool for later safe disposal
                        _trashBag.Add(_retainedSurface);
                        Debug.WriteLine("Deferred previous retained surface to pool");

                        _retainedSurface = null;
                    }

                    _retainedSurface = SKSurface.Create(_context, renderTarget, SurfaceOrigin, ColorType);
                    Debug.WriteLine("Created new retained surface");

                    _lastSize = newSize;
                    _needsFullRedraw = true;
                }

                using (new SKAutoCanvasRestore(_retainedSurface.Canvas, true))
                {
                    OnPaintSurface(new(_retainedSurface, renderTarget, SurfaceOrigin, ColorType));
                }

                using var image = _retainedSurface.Snapshot();
                using var framebufferSurface = SKSurface.Create(_context, renderTarget, SurfaceOrigin, ColorType);

                framebufferSurface.Canvas.DrawImage(image, 0, 0);
                framebufferSurface.Canvas.Flush();
                framebufferSurface.Flush();
                _context.Flush();

                _needsFullRedraw = false;
            }
        }
     

        private readonly ConcurrentBag<SKSurface> _trashBag = new();

        private bool _cleanupRunning;
        private GRBackendRenderTarget renderTarget;

        protected override void OnDestroyingContext()
        {
            base.OnDestroyingContext();

        //    lock (_surfaceLock)
            {
                renderTarget?.Dispose();

                _cleanupRunning = false;

                _retainedSurface?.Dispose();
                _retainedSurface = null;

                _context?.AbandonContext(false);
                _context?.Dispose();
                _context = null;

                _glInterface?.Dispose();
                _glInterface = null;

                _lastSize = default;
                _needsFullRedraw = true;
            }
        }

        //backup
        //protected override void OnRenderFrame(Windows.Foundation.Rect rect)
        //{

        //    // create the SkiaSharp context
        //    if (context == null)
        //    {
        //        glInterface = GRGlInterface.Create();
        //        context = GRContext.CreateGl(glInterface);
        //    }

        //    // get the new surface size
        //    var newSize = new SKSizeI((int)rect.Width, (int)rect.Height);

        //    // manage the drawing surface
        //    if (renderTarget == null || lastSize != newSize || !renderTarget.IsValid)
        //    {
        //        // create or update the dimensions
        //        lastSize = newSize;

        //        // read the info from the buffer
        //        OpenGles.GetIntegerv(OpenGles.GL_FRAMEBUFFER_BINDING, out var framebuffer);
        //        OpenGles.GetIntegerv(OpenGles.GL_STENCIL_BITS, out var stencil);
        //        OpenGles.GetIntegerv(OpenGles.GL_SAMPLES, out var samples);
        //        var maxSamples = context.GetMaxSurfaceSampleCount(colorType);
        //        if (samples > maxSamples)
        //            samples = maxSamples;

        //        glInfo = new GRGlFramebufferInfo((uint)framebuffer, colorType.ToGlSizedFormat());

        //        // destroy the old surface
        //        _surface?.Dispose();
        //        _surface = null;

        //        // re-create the render target
        //        renderTarget?.Dispose();
        //        renderTarget = new GRBackendRenderTarget(newSize.Width, newSize.Height, samples, stencil, glInfo);
        //    }

        //    // create the surface
        //    if (_surface == null)
        //    {
        //        _surface = SKSurface.Create(context, renderTarget, surfaceOrigin, colorType);

        //        // clear everything
        //        OpenGles.Clear(OpenGles.GL_COLOR_BUFFER_BIT | OpenGles.GL_DEPTH_BUFFER_BIT |
        //                       OpenGles.GL_STENCIL_BUFFER_BIT);

        //    }

        //    var canvas = _surface.Canvas;

        //    using (new SKAutoCanvasRestore(canvas, true))
        //    {
        //        // start drawing
        //        OnPaintSurface(new(_surface, renderTarget, surfaceOrigin, colorType));
        //    }

        //    // update the control
        //    canvas.Flush();
        //    _surface.Flush();
        //    context.Flush();
        //}

        //protected override void OnDestroyingContext()
        //{
        //    base.OnDestroyingContext();

        //    lastSize = default;

        //    _surface?.Dispose();
        //    _surface = null;

        //    renderTarget?.Dispose();
        //    renderTarget = null;

        //    glInfo = default;

        //    context?.AbandonContext(false);
        //    context?.Dispose();
        //    context = null;

        //    glInterface?.Dispose();
        //    glInterface = null;
        //}
    }
}
