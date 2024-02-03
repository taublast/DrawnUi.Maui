namespace DrawnUi.Maui.Controls;

public partial class MauiEditor : Editor
{
    public override SizeRequest Measure(double widthConstraint, double heightConstraint, MeasureFlags flags = MeasureFlags.None)
    {
        var ret = base.Measure(widthConstraint, heightConstraint, flags);

        return ret;
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        var ret = base.ArrangeOverride(bounds);

        return ret;
    }

}