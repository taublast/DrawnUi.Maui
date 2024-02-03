using System.Diagnostics;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Warning with CPU-rendering edges will not be blurred: https://issues.skia.org/issues/40036320
/// </summary>
public class SkiaBackdrop : ContentLayout
{

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

    public virtual void InvalidateImageFilter()
    {
        NeedInvalidateImageFilter = true;
    }

    private static void NeedChangeImageFilter(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaBackdrop control)
        {
            control.InvalidateImageFilter();
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

    public bool HasEffects
    {
        get
        {
            return Blur != 0;
        }
    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        PaintImageFilter?.Dispose();
        PaintImageFilter = null;

        ImagePaint?.Dispose();
        ImagePaint = null;
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

        if (destination.Width > 0 && destination.Height > 0)
        {
            DrawViews(ctx, destination, scale);

            PaintTintBackground(ctx.Canvas, destination);

            if (HasEffects)
            {
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

                ImagePaint.ImageFilter = PaintImageFilter;

                //notice we read from the real canvas and we write to ctx.Canvas which can be cache
                ctx.Superview.CanvasView.Surface.Canvas.Flush();

                using var snapshot = ctx.Superview.CanvasView.Surface.Snapshot(new((int)destination.Left, (int)destination.Top, (int)destination.Right, (int)destination.Bottom));

#if IOS
            //cannot really blur in realtime on GL simulator would be like 2 fps
            //while on Metal and M1 the blur will just not work
            if (DeviceInfo.Current.DeviceType != DeviceType.Virtual )
#endif
                {
                    if (snapshot != null)
                    {
                        ctx.Canvas.DrawImage(snapshot, destination, ImagePaint);
                    }

                }

            }
        }

    }


}