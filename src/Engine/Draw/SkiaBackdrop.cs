using System.Diagnostics;

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

            ctx.Superview.CanvasView.Surface.Canvas.Flush();
            var snapshot = ctx.Superview.CanvasView.Surface.Snapshot(new((int)destination.Left, (int)destination.Top, (int)destination.Right, (int)destination.Bottom));

#if IOS
            //cannot really blur in realtime on GL simulator would be like 2 fps
            //while on Metal and M1 the blur will just not work
            if (DeviceInfo.Current.DeviceType != DeviceType.Virtual )
#endif
            {
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