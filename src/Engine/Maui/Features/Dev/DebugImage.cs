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
public partial class DebugImage : SkiaShape
{
    public SkiaImage Display { get; protected set; }
    public SkiaLabel Caption { get; protected set; }

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

    protected virtual SkiaLabel CreateLabel()
    {
        return new SkiaLabel()
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            BackgroundColor = Colors.Black,
            Padding = new(4,2),
            Text = this.Text
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

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(DebugImage),
        string.Empty);

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        if (IsDisposed || IsDisposing)
            return ScaledSize.Default;

        if (Display == null)
        {
            Display = CreatePreview();
            Caption = CreateLabel();
            var layout = new SkiaLayout()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                }
                .WithChildren(Display, Caption);
            Content = layout;
        }

        return base.Measure(widthConstraint, heightConstraint, scale);
    }

    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {
        if (AttachTo != null)
        {
            var snapshot = AttachTo.Snapshot();
            Display.SetImageInternal(snapshot, true);
        }

        if (Text != Caption.Text)
        {
            Caption.Text = Text;
        }

        base.Paint(ctx, destination, scale, arguments);
    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        AttachTo = null;
        Display = null;
        Caption = null;
    }
}

