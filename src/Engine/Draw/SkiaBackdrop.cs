﻿using System.Diagnostics;

namespace DrawnUi.Maui.Draw;


/// <summary>
/// Warning with CPU-rendering edges will not be blurred: https://issues.skia.org/issues/40036320
/// </summary>
public class SkiaBackdrop : ContentLayout
{
    public override ISkiaGestureListener ProcessGestures(TouchActionType type, TouchActionEventArgs args, TouchActionResult touchAction,
        SKPoint childOffset, SKPoint childOffsetDirect, ISkiaGestureListener alreadyConsumed)
    {
        //consume everything
        return this;
    }

    /// <summary>
    /// Reusing this
    /// </summary>
    protected SKPaint ImagePaint;

    /// <summary>
    /// Reusing this
    /// </summary>
    protected SKImageFilter PaintImageFilter;

    /// <summary>
    /// Reusing this
    /// </summary>
    protected SKColorFilter PaintColorFilter;

    protected bool NeedInvalidateImageFilter { get; set; }
    protected bool NeedInvalidateColorFilter { get; set; }

    public virtual void InvalidateImageFilter()
    {
        NeedInvalidateImageFilter = true;
    }

    public virtual void InvalidateColorFilter()
    {
        NeedInvalidateColorFilter = true;
    }

    private static void NeedChangeImageFilter(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaBackdrop control)
        {
            control.InvalidateImageFilter();
            NeedDraw(bindable, oldvalue, newvalue);
        }
    }

    private static void NeedChangeColorFilter(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaBackdrop control)
        {
            control.InvalidateColorFilter();
            NeedDraw(bindable, oldvalue, newvalue);
        }
    }

    public static readonly BindableProperty BlurProperty = BindableProperty.Create(
        nameof(Blur),
        typeof(double),
        typeof(SkiaBackdrop),
        5.0,
        propertyChanged: NeedChangeImageFilter);


    public double Blur
    {
        get { return (double)GetValue(BlurProperty); }
        set { SetValue(BlurProperty, value); }
    }

    public static readonly BindableProperty BrightnessProperty = BindableProperty.Create(
        nameof(Brightness),
        typeof(double),
        typeof(SkiaBackdrop),
        1.0,
        propertyChanged: NeedChangeImageFilter);


    public double Brightness
    {
        get { return (double)GetValue(BrightnessProperty); }
        set { SetValue(BrightnessProperty, value); }
    }

    public bool HasEffects
    {
        get
        {
            return Blur != 0 || Brightness != 1;
        }
    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        PaintImageFilter?.Dispose();
        PaintImageFilter = null;

        PaintColorFilter?.Dispose();
        PaintColorFilter = null;

        ImagePaint?.Dispose();
        ImagePaint = null;

        Snapshot?.Dispose();
        Snapshot = null;
        
        if (CacheSource != null)
        {
            CacheSource.CreatedCache -= OnSourceCacheChanged;
            CacheSource = null;
        }
    }

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        if (IsDisposed)
        {
            //this will save a lot of trouble debugging unknown native crashes
            var message = $"[SkiaControl] Attempting to Paint a disposed control: {this}";
            Trace.WriteLine(message);
            throw new Exception(message);
        }

        if (NeedInvalidateImageFilter)
        {
            NeedInvalidateImageFilter = false;
            // PaintImageFilter?.Dispose(); //might be used in double buffered!
            PaintImageFilter = null;
        }

        if (NeedInvalidateColorFilter)
        {
            NeedInvalidateColorFilter = false;
            // PaintColorFilter?.Dispose(); //might be used in double buffered!
            PaintColorFilter = null;
        }

        if (destination.Width > 0 && destination.Height > 0)
        {
            DrawViews(ctx, destination, scale);

            PaintTintBackground(ctx.Canvas, destination);

            BuildPaint();

            ImagePaint.ImageFilter = PaintImageFilter;
            ImagePaint.ColorFilter = PaintColorFilter;

            //notice we read from the real canvas and we write to ctx.Canvas which can be cache

            //if (ctx.Superview.HardwareAcceleration != HardwareAccelerationMode.Disabled)
            
                if (CacheSource != null)
                {
                    var cache = CacheSource.RenderObject;
                    if (cache != null && !IsGhost && HasEffects)
                    {
                        var offsetX = CacheSource.DrawingRect.Left - this.DrawingRect.Left;
                        var offsetY = CacheSource.DrawingRect.Top - this.DrawingRect.Top;
                        destination.Offset(offsetX, offsetY);
                        
                        cache.Draw(ctx.Superview.CanvasView.Surface.Canvas, destination, ImagePaint);
                    }
                }
                else
                {
                    ctx.Superview.CanvasView.Surface.Canvas.Flush();
                    var snapshot = ctx.Superview.CanvasView.Surface.Snapshot(new((int)destination.Left, (int)destination.Top, (int)destination.Right, (int)destination.Bottom));
 
                    if (snapshot != null)
                    {
                        if (!IsGhost && HasEffects)
                            ctx.Canvas.DrawImage(snapshot, destination, ImagePaint);

                        Snapshot?.Dispose();
                        Snapshot = snapshot;
                    }
                    
                }
            
            }

    }

    protected SKImage Snapshot { get; set; }

    //for some platforms like iOS Metal we cannot get the snapshot
    // of not yet rendered content below
    // so we will use the cache
    public static readonly BindableProperty CacheSourceProperty = BindableProperty.Create(
        nameof(SkiaBackdrop),
        typeof(SkiaControl),
        typeof(SkiaBackdrop),
        null,
        propertyChanged: WhenSourceChanged,
        defaultBindingMode: BindingMode.OneTime);
    public SkiaControl CacheSource
    {
        get { return (SkiaControl)GetValue(CacheSourceProperty); }
        set { SetValue(CacheSourceProperty, value); }
    }
    private static void WhenSourceChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaBackdrop control)
        {
            control.AttachSource();
        }
    }

    /// <summary>
    /// Designed to be just one-time set
    /// </summary>
    protected void AttachSource()
    {
        if (CacheSource != null)
        {
            CacheSource.CreatedCache += OnSourceCacheChanged;
        }
    }

    private void OnSourceCacheChanged(object sender, CachedObject e)
    {
        Update();
    }
    

    /// <summary>
    /// Returns the snapshot that was used for drawing the backdrop.
    /// If we have no effects or the control has not yet been drawn the return value will be null.
    /// You are responsible to dispose the returned image yourself.
    /// </summary>
    /// <returns></returns>
    public virtual SKImage GetImage()
    {
        var image = Snapshot;
        if (image != null)
        {
            Snapshot = null; //save it from being disposed by our Paint method
            return image;
        }
        return null;
    }

    protected virtual void BuildPaint()
    {
        if (PaintColorFilter == null)
        {
            PaintColorFilter = SkiaImageEffects.Gamma((float)this.Brightness);
        }

        if (PaintImageFilter == null)
        {
            PaintImageFilter = SKImageFilter.CreateBlur((float)Blur, (float)Blur, SKShaderTileMode.Mirror);
        }

        if (ImagePaint == null)
        {
            ImagePaint = new()
            {
            };
        }
    }


}