using Microsoft.Maui.Graphics.Skia;

namespace DrawnUi.Maui.Draw;

[ContentProperty("Drawable")]
public class SkiaMauiGraphics : ContentLayout 
{

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        base.Paint(ctx, destination, scale, arguments);

        if (Drawable != null)
        {
            Canvas.Canvas = ctx.Canvas;

            Drawable.Draw(Canvas, new RectF(destination.Left, destination.Top, destination.Width, destination.Height));
        }
    }

    protected SkiaCanvas Canvas { get; set; }

    public static readonly BindableProperty DrawableProperty = BindableProperty.Create(
        nameof(Drawable),
        typeof(IDrawable),
        typeof(SkiaMauiGraphics),
        null,
        propertyChanged: OnReplaceDrawable);

    private static void OnReplaceDrawable(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaMauiGraphics control)
        {
            control.SetDrawable(newvalue as SkiaControl);
        }
    }

    private void SetDrawable(SkiaControl newvalue)
    {
        var kill = Canvas;

        if (Drawable != null)
            Canvas = new SkiaCanvas();

        kill?.Dispose();
    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        if (Drawable is IDisposable disposable)
        {
            disposable.Dispose();
        }
        Drawable = null;
        Canvas?.Dispose();
    }

    public IDrawable Drawable
    {
        get { return (IDrawable)GetValue(DrawableProperty); }
        set { SetValue(DrawableProperty, value); }
    }


}

/*
   public static class MauiGraphicsExtensions
   {
       public static SKTypeface ToSKTypeface(this IFont font)
       {
           if (string.IsNullOrEmpty(font?.Name))
               return SKTypeface.Default;

           try
           {
               return SKTypeface.FromFamilyName(font.Name, font.Weight, (int)SKFontStyleWidth.Normal,
                   font.StyleType switch
                   {
                       FontStyleType.Normal => SKFontStyleSlant.Upright,
                       FontStyleType.Italic => SKFontStyleSlant.Italic,
                       FontStyleType.Oblique => SKFontStyleSlant.Oblique,
                       _ => SKFontStyleSlant.Upright,
                   });
           }
           catch
           {
               return SKTypeface.FromFile(font.Name);
           }
       }
       public static SKPaint CreateCopy(this SKPaint paint)
       {
           if (paint == null)
               return null;

           var copy = new SKPaint
           {
               BlendMode = paint.BlendMode,
               Color = paint.Color,
               ColorFilter = paint.ColorFilter,
               ImageFilter = paint.ImageFilter,
               IsAntialias = paint.IsAntialias,
               IsStroke = paint.IsStroke,
               MaskFilter = paint.MaskFilter,
               Shader = paint.Shader,
               StrokeCap = paint.StrokeCap,
               StrokeJoin = paint.StrokeJoin,
               StrokeMiter = paint.StrokeMiter,
               StrokeWidth = paint.StrokeWidth,
               TextAlign = paint.TextAlign,
               TextEncoding = paint.TextEncoding,
               TextScaleX = paint.TextScaleX,
               TextSize = paint.TextSize,
               TextSkewX = paint.TextSkewX,
               Typeface = paint.Typeface,
           };

           return copy;
       }
   }
 */