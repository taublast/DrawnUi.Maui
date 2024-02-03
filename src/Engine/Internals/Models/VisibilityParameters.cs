namespace DrawnUi.Maui.Infrastructure;

public struct VisibilityParameters
{
    public bool IsVisible;
    public static VisibilityParameters Visible = new VisibilityParameters { IsVisible = true };
    public static VisibilityParameters Hidden = new VisibilityParameters { IsVisible = false };
}