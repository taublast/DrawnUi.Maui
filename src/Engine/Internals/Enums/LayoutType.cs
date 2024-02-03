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
    /// Use usual grid properties like Grid.Column, ColumnSpacing etc
    /// </summary>
    Grid,

    /// <summary>
    /// todo
    /// </summary>
    MasonryColumns,

    /// <summary>
    /// todo
    /// </summary>

    MasonryRows,
    /// <summary>
    /// todo
    /// </summary>
    Flex
}