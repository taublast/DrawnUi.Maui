using System.Collections.Concurrent;
using System.ComponentModel;
using Android.Content;
using Android.Opengl;
using Markdig.Helpers;
using SkiaSharp;
using static Android.Icu.Text.IDNA;
using SKPaintGLSurfaceEventArgs = SkiaSharp.Views.Android.SKPaintGLSurfaceEventArgs;

namespace DrawnUi;

public abstract partial class SkiaGLTextureRenderer : Java.Lang.Object, SkiaGLTextureView.IRenderer
{
    protected const SKColorType colorType = SKColorType.Rgba8888;
    protected const GRSurfaceOrigin surfaceOrigin = GRSurfaceOrigin.BottomLeft;
    protected GRContext context;
    protected GRGlFramebufferInfo glInfo;
    protected GRBackendRenderTarget renderTarget;
    private SKSurface surface;
    private SKCanvas canvas;
    protected SKSizeI lastSize;
    protected SKSizeI newSize;
    public SKSize CanvasSize => lastSize;
    public GRContext GRContext => context;

    protected virtual void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
    {
    }

    public virtual void OnDrawFrame()
    {
        //thank you but no
        //GLES10.GlClear(GLES10.GlColorBufferBit | GLES10.GlDepthBufferBit | GLES10.GlStencilBufferBit);

        // create the contexts if not done already
        if (context == null)
        {
            var glInterface = GRGlInterface.Create();
            context = GRContext.CreateGl(glInterface);
        }

        // manage the drawing surface
        if (renderTarget == null || lastSize != newSize || !renderTarget.IsValid)
        {
            // create or update the dimensions
            lastSize = newSize;

            // read the info from the buffer
            var buffer = new int[3];
            GLES20.GlGetIntegerv(GLES20.GlFramebufferBinding, buffer, 0);
            GLES20.GlGetIntegerv(GLES20.GlStencilBits, buffer, 1);
            GLES20.GlGetIntegerv(GLES20.GlSamples, buffer, 2);
            var samples = buffer[2];
            var maxSamples = context.GetMaxSurfaceSampleCount(colorType);
            if (samples > maxSamples)
                samples = maxSamples;
            glInfo = new GRGlFramebufferInfo((uint)buffer[0], colorType.ToGlSizedFormat());

            // destroy the old surface
            surface?.Dispose();
            surface = null;
            canvas = null;

            // re-create the render target
            renderTarget?.Dispose();
            renderTarget = new GRBackendRenderTarget(newSize.Width, newSize.Height, samples, buffer[1], glInfo);
        }

        // create the surface
        if (surface == null)
        {
            surface = SKSurface.Create(context, renderTarget, surfaceOrigin, colorType);
        }

        if (surface != null)
        {
            canvas = surface.Canvas;
        }

        if (canvas != null)
        {
            var restore = canvas.Save();

            var e = new SKPaintGLSurfaceEventArgs(surface, renderTarget, surfaceOrigin, colorType);
            OnPaintSurface(e);

            canvas.RestoreToCount(restore);

            canvas.Flush();
            context.Flush();
        }
    }

    public void OnSurfaceChanged(int width, int height)
    {
        GLES20.GlViewport(0, 0, width, height);

        // get the new surface size
        newSize = new SKSizeI(width, height);
    }

    public void OnSurfaceCreated(EGLConfig config)
    {
        // Create the context and resources
        if (context != null)
        {
            FreeContext();
        }
    }

    public void OnSurfaceDestroyed()
    {
        FreeContext();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            FreeContext();
        }

        base.Dispose(disposing);
    }

    private void FreeContext()
    {
        surface?.Dispose();
        surface = null;
        renderTarget?.Dispose();
        renderTarget = null;
        context?.Dispose();
        context = null;
    }
}

public class RetainedSkiaGLTextureRenderer : SkiaGLTextureRenderer
{
    private SKSurface _retainedSurface; // Persistent offscreen surface
    private bool _needsFullRedraw = true; // For initial frame or size change
    private readonly ConcurrentBag<SKSurface> _trashBag = new();
    private bool _cleanupRunning;

    public RetainedSkiaGLTextureRenderer()
    {
        _cleanupRunning = true;
        // Start a timer to safely dispose surfaces after they're no longer used by the GPU
        Task.Run(async () =>
        {
            while (_cleanupRunning)
            {
                await Task.Delay(2000); // 2-second delay
                while (_trashBag.TryTake(out var pooledSurface))
                {
                    // After a long delay, GPU is guaranteed done
                    pooledSurface.Dispose();
                    System.Diagnostics.Debug.WriteLine("Disposed pooled surface safely after delay");
                }
            }
        });
    }

    public override void OnDrawFrame()
    {
        // Don't clear the screen - we're going to draw our retained surface
        // GLES10.GlClear(GLES10.GlColorBufferBit | GLES10.GlDepthBufferBit | GLES10.GlStencilBufferBit);

        // Create the GL contexts if not done already
        if (context == null)
        {
            var glInterface = GRGlInterface.Create();
            context = GRContext.CreateGl(glInterface);
        }

        // Manage the drawing surface
        if (renderTarget == null || lastSize != newSize || !renderTarget.IsValid)
        {
            // Create or update the dimensions
            lastSize = newSize;

            // Read the info from the buffer
            var buffer = new int[3];
            GLES20.GlGetIntegerv(GLES20.GlFramebufferBinding, buffer, 0);
            GLES20.GlGetIntegerv(GLES20.GlStencilBits, buffer, 1);
            GLES20.GlGetIntegerv(GLES20.GlSamples, buffer, 2);
            var samples = buffer[2];
            var maxSamples = context.GetMaxSurfaceSampleCount(colorType);
            if (samples > maxSamples)
                samples = maxSamples;
            glInfo = new GRGlFramebufferInfo((uint)buffer[0], colorType.ToGlSizedFormat());

            // Handle the retained surface - add to trash bag for later disposal
            if (_retainedSurface != null)
            {
                _retainedSurface.Canvas.Flush();
                _retainedSurface.Flush();
                context.Flush();
                _trashBag.Add(_retainedSurface);
                _retainedSurface = null;
            }

            // Re-create the render target
            renderTarget?.Dispose();
            renderTarget = new GRBackendRenderTarget(newSize.Width, newSize.Height, samples, buffer[1], glInfo);

            _needsFullRedraw = true;
        }

        // Create the retained surface if needed
        if (_retainedSurface == null)
        {
            _retainedSurface = SKSurface.Create(context, renderTarget, surfaceOrigin, colorType);
            _needsFullRedraw = true;
        }

        // If we need a full redraw (new surface or size changed)
        if (_needsFullRedraw)
        {
            // Clear the retained surface
            _retainedSurface.Canvas.Clear(SKColors.Transparent);
        }

        // Draw to our retained surface
        using (new SKAutoCanvasRestore(_retainedSurface.Canvas, true))
        {
            var e = new SKPaintGLSurfaceEventArgs(_retainedSurface, renderTarget, surfaceOrigin, colorType);
            OnPaintSurface(e);
        }

        // Now create a temporary surface for the actual framebuffer
        using (var framebufferSurface = SKSurface.Create(context, renderTarget, surfaceOrigin, colorType))
        {
            // Draw the retained surface to the framebuffer
            using (var image = _retainedSurface.Snapshot())
            {
                framebufferSurface.Canvas.DrawImage(image, 0, 0);
            }

            framebufferSurface.Canvas.Flush();
            framebufferSurface.Flush();
        }

        context.Flush();
        _needsFullRedraw = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cleanupRunning = false;
            _retainedSurface?.Dispose();
            _retainedSurface = null;
        }

        base.Dispose(disposing);
    }
}
