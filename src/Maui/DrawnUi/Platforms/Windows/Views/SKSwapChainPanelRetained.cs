using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Application = Microsoft.Maui.Controls.Application;

namespace DrawnUi.Views
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
        public Guid Uid = Guid.CreateVersion7();

        private const SKColorType ColorType = SKColorType.Rgba8888;
        private const GRSurfaceOrigin SurfaceOrigin = GRSurfaceOrigin.BottomLeft;
        private GRGlInterface _glInterface;
        private GRContext _context;
        private SKSurface _retainedSurface; // Persistent offscreen surface
        private SKSizeI _lastSize;
        private bool _needsFullRedraw = true; // For initial frame or size change

        private readonly object _surfaceLock = new();

        // Track surfaces with their disposal time
        private readonly ConcurrentDictionary<SKSurface, DateTime> _trashBag = new();
        private bool _cleanupRunning;
        private GRBackendRenderTarget renderTarget;

        public SKSwapChainPanelRetained()
        {
            _cleanupRunning = true;
            Tasks.StartTimerAsync(TimeSpan.FromSeconds(0.5), async () =>
            {
                var now = DateTime.Now;
                var disposalAge = TimeSpan.FromSeconds(2);

                // Get all surfaces older than 2 seconds
                var oldSurfaces = _trashBag
                    .Where(kvp => (now - kvp.Value) >= disposalAge)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var surface in oldSurfaces)
                {
                    if (_trashBag.TryRemove(surface, out _))
                    {
                        surface.Dispose();
                        Debug.WriteLine($"Disposed pooled surface safely after {disposalAge.TotalSeconds} seconds");
                    }
                }

                return _cleanupRunning;
            });
        }

        public SKSize CanvasSize => _lastSize;
        public GRContext GRContext => _context;
        public event EventHandler<SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs> PaintSurface;

        /// <summary>
        /// Raises the PaintSurface event
        /// </summary>
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

                        // Add the surface to the trash bag with the current timestamp
                        _trashBag[_retainedSurface] = DateTime.Now;
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

        protected override void OnDestroyingContext()
        {
            base.OnDestroyingContext();
            foreach (var surface in _trashBag.Keys.ToList())
            {
                if (_trashBag.TryRemove(surface, out _))
                {
                    surface.Dispose();
                }
            }

            //without this we'll fall into gpu backbuffer still using this context
            Tasks.StartDelayed(TimeSpan.FromMilliseconds(2500), () =>
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

            });
        }
    }
}
