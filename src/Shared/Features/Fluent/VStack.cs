namespace DrawnUi.Draw;

/// <summary>
/// Vertical stack, like MAUI VerticalStackLayout
/// </summary>
public partial class VStack : SkiaLayout
{
    public VStack()
    {
        Type = LayoutType.Column;
        HorizontalOptions = LayoutOptions.Fill;
    }
}
