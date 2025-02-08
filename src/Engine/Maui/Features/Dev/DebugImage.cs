using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawnUi.Maui.Draw;

/// <summary>
/// Control for displaying used Surface as a preview image, for debugging purposes.
/// Do not use this in prod, this will be invalidated every frame, causing non-stop screen update.
/// </summary>
public partial class DebugImage : SkiaControl
{
    public SkiaImage Display { get; protected set; }

    protected virtual SkiaImage CreatePreview()
    {
        return new SkiaImage()
        {
            LoadSourceOnFirstDraw = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.DarkGrey, 
            Aspect = TransformAspect.AspectFitFill,
        };
    }


    public static readonly BindableProperty AttachToProperty = BindableProperty.Create(
        nameof(AttachTo),
        typeof(SKSurface),
        typeof(DebugImage),
        null);

    public SKSurface AttachTo
    {
        get { return (SKSurface)GetValue(AttachToProperty); }
        set { SetValue(AttachToProperty, value); }
    }


    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        if (IsDisposed || IsDisposing)
            return ScaledSize.Default;

        if (Display == null)
        {
            //will serve as preview wrapper
            Display = CreatePreview();
            Display.SetParent(this);
        }

        return base.Measure(widthConstraint, heightConstraint, scale);
    }

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        base.Paint(ctx, destination, scale, arguments);

        if (AttachTo != null)
        {
            var snapshot = AttachTo.Snapshot();
            Display.SetImageInternal(snapshot, true);
        }

        DrawViews(ctx, DrawingRect, scale);
    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        AttachTo = null;
    }
}

