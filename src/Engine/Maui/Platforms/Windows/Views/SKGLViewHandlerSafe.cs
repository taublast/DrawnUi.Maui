using System.Collections.Concurrent;
using SkiaSharp.Views.Maui.Handlers;
using SkiaSharp.Views.Windows;
using Rect = Windows.Foundation.Rect;
using SKPaintGLSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs;

namespace DrawnUi.Maui.Views
{

    public partial class SKGLViewHandlerSafe : SKGLViewHandler
    {
        protected override SKSwapChainPanel CreatePlatformView()
        {
            return new SKSwapChainPanelFixed();
        }
    }

    public class SKSwapChainPanelFixed : SKSwapChainPanel
    {
        public bool IgnorePixelScaling { get; set; }
        public new SKSize CanvasSize => lastSize;
        public new GRContext GRContext => context;
        private const SKColorType colorType = SKColorType.Rgba8888;
        private const GRSurfaceOrigin surfaceOrigin = GRSurfaceOrigin.BottomLeft;
        private GRGlInterface glInterface;
        private GRContext context;
        private GRGlFramebufferInfo glInfo;
        private GRBackendRenderTarget renderTarget;
        private SKSurface surface;
        private SKSizeI lastSize;
        private bool cleanupRunning;
        private readonly ConcurrentBag<SKSurface> trashBag = new();

        public SKSwapChainPanelFixed()
        {
            cleanupRunning = true;
            Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                DisposeSafely();

                return cleanupRunning;
            });
        }

        void DisposeSafely()
        {
            while (trashBag.TryTake(out var pooledSurface))
            {
                pooledSurface.Dispose();
                Debug.WriteLine("Disposed pooled surface safely");
            }
        }

        protected override void OnRenderFrame(Rect rect)
        {

            if (context == null)
            {
                glInterface = GRGlInterface.Create();
                context = GRContext.CreateGl(glInterface);
            }

            var newSize = new SKSizeI((int)rect.Width, (int)rect.Height);

            bool needNewSurfaces = surface == null || lastSize != newSize;

            if (lastSize != newSize || renderTarget == null || !renderTarget.IsValid)
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
                var maxSamples = context.GetMaxSurfaceSampleCount(colorType);
                samples = Math.Min(samples, maxSamples);
                var glInfo = new GRGlFramebufferInfo((uint)framebuffer, colorType.ToGlSizedFormat());

                renderTarget =
                    new GRBackendRenderTarget(newSize.Width, newSize.Height, samples, stencil, glInfo);

                needNewSurfaces = true;
            }

            // read the info from the buffer
            if (needNewSurfaces)
            {
                if (surface != null)
                {
                    // destroy the old surface
                    Debug.WriteLine("Abandoned surface to pool");
                    trashBag.Add(surface);
                    surface = null;
                }

                surface = SKSurface.Create(context, renderTarget, surfaceOrigin, colorType);
                Debug.WriteLine("Created new surface");

                lastSize = newSize;
            }

            using (new SKAutoCanvasRestore(surface.Canvas, true))
            {
                // start drawing
                OnPaintSurface(new(surface, renderTarget, surfaceOrigin, colorType));
            }

            // update the control
            surface.Flush();
            context.Flush();
        }

        protected override void OnPaintSurface(SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs e)
        {
            if (IgnorePixelScaling)
            {
                var density = (float)ContentsScale;
                var userVisibleSize = new SKSizeI((int)(e.Info.Width / density), (int)(e.Info.Height / density));
                var canvas = e.Surface.Canvas;
                canvas.Scale(density);
                canvas.Save();

                e = new(e.Surface, e.BackendRenderTarget, e.Origin, e.Info.WithSize(userVisibleSize), e.Info);
            }

            base.OnPaintSurface(e);
        }

        protected override void OnDestroyingContext()
        {
            base.OnDestroyingContext();

            renderTarget?.Dispose();

            cleanupRunning = false;

            surface?.Dispose();
            surface = null;

            context?.AbandonContext(false);
            context?.Dispose();
            context = null;

            glInterface?.Dispose();
            glInterface = null;

            lastSize = default;

            Application.Current?.Dispatcher?.StartTimer(TimeSpan.FromSeconds(2), () =>
            {
                DisposeSafely();

                return true;
            });
        }
    }

}
