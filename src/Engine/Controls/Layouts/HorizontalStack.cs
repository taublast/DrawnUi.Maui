namespace DrawnUi.Maui.Controls;

/// <summary>
/// Helper class for SkiaLayout Type = LayoutType.Stack,  SplitMax = 0
/// </summary>
public class HorizontalStack : SkiaLayout
{
    public HorizontalStack()
    {
        Type = LayoutType.Stack;
        Split = 0;
    }
}