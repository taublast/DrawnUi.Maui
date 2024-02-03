namespace DrawnUi.Maui.Draw;

[ContentProperty(nameof(Tag))]
public class FindTagExtension : IMarkupExtension
{
    public string Tag { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        // Retrieve the root object
        // It will be null if the tree has not yet been constructed..
        var rootObjectProvider = serviceProvider.GetService<IRootObjectProvider>();
        if (rootObjectProvider != null)
        {
            var skiaControl = rootObjectProvider.RootObject as SkiaControl;
            if (skiaControl == null)
                throw new ArgumentNullException(nameof(skiaControl));

            return skiaControl.FindViewByTag(Tag);
        }

        return null;
    }
}