namespace DrawnUi.Maui.Controls;

public class ContentWithBackdrop : ContentLayout
{
    public SkiaBackdrop Backdrop { get; protected set; }

    public bool NoBackdrop { get; set; }

    protected override void SetContent(SkiaControl view)
    {
        base.SetContent(view);

        if (Backdrop == null && !NoBackdrop)
        {
            Backdrop = new SkiaBackdrop()
            {
                Tag = "backdrop",
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                ZIndex = -1,
            };
            if (Views.All(x => x != Backdrop))
            {
                AddSubView(Backdrop);
            }
        }
    }

    protected override int DrawViews(SkiaDrawingContext context, SKRect destination, float scale, bool debug = false)
    {
        if (context.Superview == null || destination.Width <= 0 || destination.Height <= 0)
        {
            return 0;
        }

        var drawViews = GetOrderedSubviews();
        return RenderViewsList(drawViews, context, destination, scale);
    }
}
