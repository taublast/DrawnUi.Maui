namespace DrawnUi.Maui.Controls;

/// <summary>
/// Helper class for SkiaLayout Type = LayoutType.Stack,  SplitMax = 1
/// </summary>
public class VStack : SkiaLayout
{
    public VStack()
    {
        Type = LayoutType.Stack;
        Split = 1;
    }
}