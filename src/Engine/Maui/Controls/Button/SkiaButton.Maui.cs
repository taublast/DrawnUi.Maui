namespace DrawnUi.Maui.Draw;

/// <summary>
/// Button-like control, can include any content inside. It's either you use default content (todo templates?..)
/// or can include any content inside, and properties will by applied by convention to a SkiaLabel with Tag `MainLabel`, SkiaShape with Tag `MainFrame`. At the same time you can override ApplyProperties() and apply them to your content yourself.
/// </summary>
public partial class SkiaButton : SkiaLayout, ISkiaGestureListener
{
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(SkiaButton),
        new CornerRadius(8),
        propertyChanged: NeedApplyProperties);

    [System.ComponentModel.TypeConverter(typeof(Microsoft.Maui.Converters.CornerRadiusTypeConverter))]
    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    protected Color InitialBackgroundColor;
    protected Brush InitialBackground;

}
