namespace DrawnUi.Controls;

/// <summary>
/// This cell can watch binding context property changing
/// </summary>
public class SkiaDynamicDrawnCell : SkiaDrawnCell
{
    protected override void OnMeasured()
    {
        base.OnMeasured();

        if (WasMeasured && MeasuredSize.Pixels != LastMeasuredSizePixels)
        {
            LastMeasuredSizePixels = MeasuredSize.Pixels;
            InvalidateChildrenTree();
        }
    }

    protected SKSize LastMeasuredSizePixels = new SKSize(-1, -1);

    protected override void FreeContext()
    {
        if (Context != null)
        {
            Context.PropertyChanged -= ContextPropertyChanged;
        }
        base.FreeContext();
    }

    protected override void AttachContext(object ctx)
    {
        base.AttachContext(ctx);

        if (Context != null)
            Context.PropertyChanged += ContextPropertyChanged;
    }

    protected virtual void ContextPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        //if (e.PropertyName == nameof(OptionItem.Selected) || e.PropertyName == nameof(OptionItem.Title))
        //{
        //	SetContent();
        //}
    }
}
