namespace DrawnUi.Maui.Draw;

/// <summary>
/// This was used to include skiacontrols being bindableobjects inside maui xaml, not really used anymore now that skiacontrol is derived from visualelement and goes okay with maui xaml out of the box..
/// </summary>
[ContentProperty("Content")]
public class ElementAdapter : View, ISkiaAttachable, IVisualTreeElement
{

    /// <summary>
    /// ISkiaAttachable implementation
    /// </summary>
    public SkiaControl AttachControl
    {
        get
        {
            return Content;
        }
    }

    protected virtual void OnContentSet(SkiaControl content)
    {
        if (content != null && BindingContext != null)
        {
            content.BindingContext = BindingContext;
        }
    }
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (this.Content != null && BindingContext != null)
        {
            Content.BindingContext = BindingContext;
        }
    }

    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(SkiaControl), typeof(ElementAdapter),
        null,
        propertyChanged: OnReplaceContent);

    private static void OnReplaceContent(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ElementAdapter control)
        {
            control.OnContentSet(newvalue as SkiaControl);
        }
    }
    public SkiaControl Content
    {
        get { return (SkiaControl)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }


    #region IVisualTreeElement

    //normally this control must not be present in the visual tree,
    //just in case:

    public virtual IReadOnlyList<IVisualTreeElement> GetVisualChildren()
    {
        return new List<IVisualTreeElement>()
        {
            Content
        };
    }

    public IVisualTreeElement GetVisualParent()
    {
        return Parent;
    }

    #endregion

}