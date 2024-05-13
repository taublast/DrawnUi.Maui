namespace DrawnUi.Maui.Draw;

public enum LayoutType
{
    /// <summary>
    /// Fastest rendering
    /// </summary>
    Absolute,

    /// <summary>
    /// Vertical stack
    /// </summary>
    Column,

    /// <summary>
    /// Horizontal stack
    /// </summary>
    Row,

    /// <summary>
    /// Stack panel
    /// </summary>
    Stack,

    /// <summary>
    /// Use usual grid properties like Grid.Stack, ColumnSpacing etc
    /// </summary>
    Grid,

    /// <summary>
    /// TODO
    /// </summary>
    Masonry,
}